using ExchangeRateApi.Resources;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    /// <summary>
    /// Exception for when the settings with the name of the AlpaVantage API environment variable
    /// is not found.
    /// </summary>
    public class AlphaVantageApiKeySettingsNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaVantageApiKeySettingsNotFoundException"/> with the name of the environment key in the settings.
        /// </summary>
        /// <param name="envKeyName">Key value of the AlphaVantage API environment key name in appsettings.json.</param>
        public AlphaVantageApiKeySettingsNotFoundException(string envKeyName)
        : base(ApiMessages.Error_EnvKeyNotFound.FormatWith(envKeyName))
        {
        }
    }
}
