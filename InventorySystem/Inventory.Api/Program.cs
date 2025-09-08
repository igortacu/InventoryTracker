using System.Reflection;
using Inventory.Application.Common;
using Inventory.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Bind only to port 5148
builder.WebHost.UseUrls("http://localhost:5148");

// EF Core (reads ConnectionStrings:Default; env var ConnectionStrings__Default works too)
builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IInventoryDbContext>(sp =>
    sp.GetRequiredService<InventoryDbContext>());

// MediatR v13
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("Inventory.Application")));

// Swagger + CORS (dev)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o =>
    o.AddPolicy("dev", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors("dev");
app.UseSwagger();
app.UseSwaggerUI();

// Health
app.MapGet("/", () => "Inventory API");

// Items endpoints (CQRS)
app.MapPost("/items", async (Inventory.Application.Items.Commands.CreateItem cmd, IMediator m) =>
{
    var id = await m.Send(cmd);
    return Results.Created($"/items/{id}", new { id });
});

app.MapPut("/items/{id:guid}", async (Guid id, Inventory.Application.Items.Commands.UpdateItem body, IMediator m) =>
{
    var ok = await m.Send(body with { Id = id });
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/items/{id:guid}", async (Guid id, IMediator m) =>
{
    var ok = await m.Send(new Inventory.Application.Items.Commands.DeleteItem(id));
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/items/{id:guid}/adjust", async (Guid id, int delta, IMediator m) =>
{
    var qty = await m.Send(new Inventory.Application.Items.Commands.AdjustQty(id, delta));
    return qty is int q ? Results.Ok(new { quantity = q }) : Results.NotFound();
});

app.MapGet("/items/{id:guid}", async (Guid id, IMediator m) =>
    await m.Send(new Inventory.Application.Items.Queries.GetItemById(id)) is { } dto
        ? Results.Ok(dto)
        : Results.NotFound());

// Accept missing query params with defaults
app.MapGet("/items", async (string? search, int? page, int? pageSize, IMediator m) =>
{
    var result = await m.Send(new Inventory.Application.Items.Queries.ListItems(
        search, page ?? 1, pageSize ?? 20));
    return Results.Ok(result);
});

app.Run();
