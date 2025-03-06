﻿using ExchangeRateApi.Models;
using ExchangeRateApi.Resources;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateApi.Data
{
    /// <summary>
    /// Database context for the Exchange API database.
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="loggerFactory">Injected logger service.</param>
    public class ExchangeRateApiDbContext : DbContext
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public ExchangeRateApiDbContext() : base() { }

        public ExchangeRateApiDbContext(DbContextOptions<ExchangeRateApiDbContext> options, IConfiguration configuration, ILoggerFactory loggerFactory) : base(options)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates database schema and seeds initial data.
        /// </summary>
        /// <param name="modelBuilder">The model builder to define the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set default value for the ExchangeRate LastUpdate row to the current time in UTC
            modelBuilder.Entity<ExchangeRate>()
                            .Property(e => e.LastUpdate)
                            .HasDefaultValueSql("GETUTCDATE()");

            // Populates ExchangeRates table with this EUR -> USD entry
            modelBuilder.Entity<ExchangeRate>().HasData(new ExchangeRate
            {
                ExchangeRateId = -1, // Placeholder ID
                FromCurrency = "EUR",
                ToCurrency = "USD",
                Bid = 1.05m,
                Ask = 1.55m,
                LastUpdate = DateTime.UtcNow
            });

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Configures the database connection using the connection string from an environment variable.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);

            // In case this is a in-memory unit test
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            string settingsKey = "EnvVarKeys:ExchangeApiDb";
            var exchangeApiDbConnectionKey = configuration.GetValue<string>(settingsKey);

            // If the environment variable key is not in the appsettings.json, stop program from running
            if (string.IsNullOrEmpty(exchangeApiDbConnectionKey))
            {
                string errorMessage = ApiMessages.Error_EnvKeyNotFound.FormatWith(settingsKey);
                loggerFactory.CreateLogger<ExchangeRateApiDbContext>().LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Retrieves the connection string from a env variable (different for each env)
            var connectionString = Environment.GetEnvironmentVariable(exchangeApiDbConnectionKey);

            // If the connection string is not set in the environment variable, stop program from running
            if (string.IsNullOrEmpty(connectionString))
            {
                loggerFactory.CreateLogger<ExchangeRateApiDbContext>().LogError(ApiMessages.Error_ConnectionStringNotFound.FormatWith(exchangeApiDbConnectionKey));
                throw new InvalidOperationException("");
            }

            loggerFactory.CreateLogger<ExchangeRateApiDbContext>().LogInformation(ApiMessages.Info_ConnectionStringFound.FormatWith(exchangeApiDbConnectionKey));
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
