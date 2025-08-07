using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Challenge_Fambec.Client;
using Challenge_Fambec.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure base address based on environment
var baseAddress = builder.HostEnvironment.IsDevelopment() 
    ? new Uri("http://localhost:5214/")
    : new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAddress });

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

await builder.Build().RunAsync();
