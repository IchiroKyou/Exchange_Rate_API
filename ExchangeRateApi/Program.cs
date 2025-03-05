using ExchangeRateApi.Filters;
using ExchangeRateApi.Models.MappingProfiles;
using ExchangeRateApi.Services;
using ExchangeRateApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient(); // Inject httpclient for the AlphaVantageService
builder.Services.AddScoped<IAlphaVantageService, AlphaVantageService>(); // Inject AlphaVantageService
builder.Services.AddControllers(options => options.Filters.Add(new ValidateModelStateAttribute())); //Adds controllers and a filter for validating the model before the action is executed.
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(ExchangeRateProfile)); // Adds automapper for the DTOs

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
