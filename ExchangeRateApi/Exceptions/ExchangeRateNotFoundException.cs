using ExchangeRateApi.Resources;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    /// <summary>
    /// Exception for when an exchange rate is not found for the specified currencies.
    /// </summary>
    public class ExchangeRateNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateNotFoundException"/> class with the currency codes.
        /// </summary>
        /// <param name="fromCurrency">The currency code that was being converted from.</param>
        /// <param name="toCurrency">The currency code that was being converted to.</param>
        public ExchangeRateNotFoundException(string fromCurrency, string toCurrency)
            : base(ApiMessages.Exception_ExchangeRateNotFound_Message.FormatWith(fromCurrency, toCurrency))
        {
        }
    }
}
