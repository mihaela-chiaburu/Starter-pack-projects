using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterPack.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddRazorPages(); 
builder.Services.AddHttpClient(); // Required for WeatherService
builder.Services.AddScoped<WeatherService>(); // Register WeatherService
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<CalculatorService>(); // Register CalculatorService

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files (e.g., css, images) from wwwroot
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages(); // Map Razor Pages routes

app.Run();