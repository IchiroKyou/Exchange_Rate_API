using ExchangeRateApi.Resources;
using ExchangeRateApi.Services.Interfaces;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    /// <summary>
    /// Exception for when the AlpaVantage API key was not found in the environmnet variable.
    /// </summary>
    public class AlphaVantageApiKeyNotFoundException : Exception
    {
        public AlphaVantageApiKeyNotFoundException()
            : base(ApiMessages.Error_ApiKeyNotFound.FormatWith(nameof(IAlphaVantageService)))
        {
        }

        public AlphaVantageApiKeyNotFoundException(Exception? innerException)
            : base(ApiMessages.Error_ApiKeyNotFound.FormatWith(nameof(IAlphaVantageService)), innerException)
        {
        }
    }
}
