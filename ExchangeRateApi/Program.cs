using ExchangeRateApi.Data;
using ExchangeRateApi.Filters;
using ExchangeRateApi.Models.MappingProfiles;
using ExchangeRateApi.Services;
using ExchangeRateApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ExchangeRateApiDbContext>();
builder.Services.AddHttpClient(); // Inject httpclient for the AlphaVantageService
builder.Services.AddControllers(options => options.Filters.Add(new ValidateModelStateAttribute())); //Adds controllers and a filter for validating the model before the action is executed.
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ExchangeRateProfile)); // Adds automapper for the DTOs
builder.Services.AddScoped<IAlphaVantageService, AlphaVantageService>(); // Inject AlphaVantageService
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>(); // Inject ExchangeRateService

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

ApplyDatabaseMigrations();

app.Run();


void ApplyDatabaseMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ExchangeRateApiDbContext>();

            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                context.Database.Migrate(); // Apply pending migrations
            }
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>(); // Gets the logger to use in the ApplyDatabaseMigrations() method
            logger.LogError(ex, $"Error applying migrations to database: {ex.Message}");
            throw; // Crash application at start, if migrations fail to apply
        }
    }
}