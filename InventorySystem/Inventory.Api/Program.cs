using System.Net;
using System.Reflection;
using Inventory.Application.Items.Commands;
using Inventory.Infrastructure;
using Inventory.Application;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// hard-bind Kestrel
builder.WebHost.ConfigureKestrel(o => o.Listen(IPAddress.Any, 5148));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("dev", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var conn =
    builder.Configuration.GetConnectionString("Default") ??
    builder.Configuration["ConnectionStrings:Default"] ??
    "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;";
builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(conn));
builder.Services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateItem))!));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Inventory.Infrastructure.InventoryDbContext>();
    await db.Database.MigrateAsync();
}


// override any URLS/HTTP_PORTS that sneak in
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5148");

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("dev");

app.MapGet("/", () => Results.Text("Inventory API", "text/plain"));
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/healthz/db", async (InventoryDbContext db) =>
    (await db.Database.CanConnectAsync()) ? Results.Ok(new { db = "ok" }) : Results.StatusCode(500));

// items
app.MapGet("/items",
    async (string? search, int? page, int? pageSize, IMediator m) =>
        Results.Ok(await m.Send(new Inventory.Application.Items.Queries.ListItems(search, page ?? 1, pageSize ?? 20))));

app.MapGet("/items/{id:guid}",
    async (Guid id, IMediator m) =>
        (await m.Send(new Inventory.Application.Items.Queries.GetItemById(id))) is { } dto
            ? Results.Ok(dto) : Results.NotFound());

app.MapPost("/items",
    async (Inventory.Application.Items.Commands.CreateItem req, IMediator m) =>
    {
        var id = await m.Send(req);
        return Results.Created($"/items/{id}", new { id });
    });

app.MapPut("/items/{id:guid}",
    async (Guid id, Inventory.Application.Items.Commands.UpdateItem req, IMediator m) =>
        (await m.Send(req with { Id = id })) ? Results.NoContent() : Results.NotFound());

app.MapDelete("/items/{id:guid}",
    async (Guid id, IMediator m) =>
        (await m.Send(new Inventory.Application.Items.Commands.DeleteItem(id))) ? Results.NoContent() : Results.NotFound());

app.MapPost("/items/{id:guid}/adjust",
    async (Guid id, int delta, IMediator m) =>
    {
        var qty = await m.Send(new Inventory.Application.Items.Commands.AdjustQty(id, delta));
        return qty is null ? Results.NotFound() : Results.Ok(new { quantity = qty });
    });

app.Run();
