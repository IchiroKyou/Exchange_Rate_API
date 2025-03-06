using ExchangeRateApi.Resources;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    /// <summary>
    /// Exception for when an new exchange rate already exists in the database.
    /// </summary>
    public class ExchangeRateAlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateAlreadyExistsException"/> class with the currency codes.
        /// </summary>
        /// <param name="fromCurrency">The currency code that was being converted from.</param>
        /// <param name="toCurrency">The currency code that was being converted to.</param>
        public ExchangeRateAlreadyExistsException(string fromCurrency, string toCurrency)
            : base(ApiMessages.Error_ExchangeRateAlreadyExists.FormatWith(fromCurrency, toCurrency))
        {
        }
    }
}
