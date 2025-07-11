using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterPack.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddRazorPages(); // Add Razor Pages support
builder.Services.AddHttpClient(); // Required for WeatherService
builder.Services.AddScoped<WeatherService>(); // Register WeatherService
builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Ensure IConfiguration is available

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Handle errors in production
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files (e.g., css, images) from wwwroot
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages(); // Map Razor Pages routes

app.Run();