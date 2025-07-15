using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterPack.Interfaces;
using StarterPack.Services;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICalculatorService, CalculatorService>();
builder.Services.AddSingleton<IToDoService, ToDoService>();
builder.Services.AddScoped<IWeatherIconService, WeatherIconService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();