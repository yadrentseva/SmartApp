using Microsoft.Extensions.Configuration;
using SmartApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// builder.Services.AddHostedService<LoadingService>();

builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "Logger.txt"));
builder.Logging.AddConsole();

var sectionSettingsApp = builder.Configuration.GetSection("SettingsApp");
var smartConfig = sectionSettingsApp.Get<SmartConfig>(); 
builder.Services.AddSingleton(smartConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
