using Demo.AppCore.Interfaces;
using Demo.AppCore.Models;
using Demo.AppCore.Services;
using Demo.Database.Context;
using Demo.Database.Services;
using Demo.Notification.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Port=65432;Database=demounittest01;User ID=admin;Password=admin;Include Error Detail=true;";

builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure grade calculation settings
builder.Services.Configure<GradeConfiguration>(options =>
{
    options.AThreshold = 90;
    options.BThreshold = 80;
    options.CThreshold = 70;
    options.DThreshold = 60;
});

// Register services
builder.Services.AddScoped<IStudentService, StudentRepository>();
builder.Services.AddScoped<IGradeCalculationService, GradeCalculationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Optional API key authentication
var apiKeyRequired = builder.Configuration.GetValue<bool>("ApiKeyRequired");
if (apiKeyRequired)
{
    builder.Services.AddAuthentication("ApiKey")
        .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, Demo.Api.Authentication.ApiKeyAuthenticationHandler>("ApiKey", null);
    builder.Services.AddAuthorization();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

if (apiKeyRequired)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

// Migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    context.Database.Migrate();
}

app.Run();

// Make Program class accessible for testing
public partial class Program { }
