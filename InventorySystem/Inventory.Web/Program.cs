using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Inventory.Web;
using Inventory.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API base URL
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5148/") });

// Services
builder.Services.AddScoped<ItemsClient>();

await builder.Build().RunAsync();
