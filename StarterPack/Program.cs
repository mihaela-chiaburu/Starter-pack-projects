using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterPack.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers(); 
builder.Services.AddHttpClient();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddScoped<CalculatorService>();
builder.Services.AddSingleton<ToDoService>(); 

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