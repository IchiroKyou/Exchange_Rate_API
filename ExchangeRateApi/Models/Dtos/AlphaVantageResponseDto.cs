using System.Text.Json.Serialization;

namespace ExchangeRateApi.Models.Dtos
{
    /// <summary>
    /// Response DTO for the AlphaVantage CURRENCY_EXCHANGE_RATE API function.
    /// 
    /// This is the structure of the response from the API (as of Feb 2025):
    /// {
    ///  "Realtime Currency Exchange Rate": {
    ///    "1. From_Currency Code": "USD",
    ///    "2. From_Currency Name": "United States Dollar",
    ///    "3. To_Currency Code": "JPY",
    ///    "4. To_Currency Name": "Japanese Yen",
    ///    "5. Exchange Rate": "148.85700000",
    ///    "6. Last Refreshed": "2025-02-26 19:14:34",
    ///    "7. Time Zone": "UTC",
    ///    "8. Bid Price": "148.85480000",
    ///    "9. Ask Price": "148.85910000"
    ///  }
    /// }
    /// </summary>
    public class AlphaVantageResponseDto
    {
        [JsonPropertyName("Realtime Currency Exchange Rate")]
        public RealtimeCurrencyExchangeRate RealtimeCurrencyExchangeRate { get; set; }
    }

    public class RealtimeCurrencyExchangeRate
    {
        [JsonPropertyName("1. From_Currency Code")]
        public string FromCurrencyCode { get; set; }

        [JsonPropertyName("2. From_Currency Name")]
        public string FromCurrencyName { get; set; }

        [JsonPropertyName("3. To_Currency Code")]
        public string ToCurrencyCode { get; set; }

        [JsonPropertyName("4. To_Currency Name")]
        public string ToCurrencyName { get; set; }

        [JsonPropertyName("5. Exchange Rate")]
        public string ExchangeRate { get; set; }

        [JsonPropertyName("6. Last Refreshed")]
        public string LastRefreshed { get; set; }

        [JsonPropertyName("7. Time Zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("8. Bid Price")]
        public string BidPrice { get; set; }

        [JsonPropertyName("9. Ask Price")]
        public string AskPrice { get; set; }
    }
}
