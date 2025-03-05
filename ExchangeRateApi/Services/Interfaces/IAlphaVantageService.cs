using ExchangeRateApi.Models.Dtos;

namespace ExchangeRateApi.Services.Interfaces
{
    /// <summary>
    /// Service for fetching data from the external Alpha Vantage API.
    /// </summary>
    public interface IAlphaVantageService
    {
        /// <summary>
        /// Retrieves the exchange rate between two currencies from the CURRENCY_EXCHANGE_RATE function of the AlphaVantage API.
        /// </summary>
        /// <param name="from">The currency code to convert from (e.g., "USD").</param>
        /// <param name="to">The currency code to convert to (e.g., "EUR").</param>
        /// <returns>A <see cref="ExchangeRateDto"/> containing the exchange rate information.</returns>
        /// <exception cref="AlphaVantageApiKeySettingsNotFound">Thrown when the Alpha Vantage API key setting is not found in the configuration.</exception>
        /// <exception cref="AlphaVantageApiKeyNotFound">Thrown when the Alpha Vantage API key is not found in the environment variables.</exception>
        /// <exception cref="ExchangeRateNotFoundException">Thrown when the specified currency codes are invalid or the exchange rate cannot be found in the Alpha Vantage API response.</exception>
        /// <exception cref="HttpRequestException">Thrown when an error occurs while communicating with the Alpha Vantage API.</exception>
        /// <exception cref="JsonException">Thrown when an error occurs while parsing the JSON response from the Alpha Vantage API.</exception>
        /// <exception cref="Exception">Thrown for any other unexpected exceptions.</exception>
        /// <remarks>
        /// This method fetches the exchange rate from the Alpha Vantage API using the provided currency codes. 
        /// It retrieves the API key from environment variables, which is configured via appsettings.json.
        /// If the API key or the exchange rate is not found, or if an error occurs during the API call or JSON parsing, an exception is thrown.
        /// </remarks>
        Task<ExchangeRateDto> GetExchangeRateAsync(string from, string to);
    }
}
