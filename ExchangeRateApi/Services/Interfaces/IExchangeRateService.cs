using ExchangeRateApi.Models.Dtos;

namespace ExchangeRateApi.Services.Interfaces
{
    /// <summary>
    /// Service that does CRUD operations over Exchange Rates
    /// </summary>
    public interface IExchangeRateService
    {
        /// <summary>
        /// Service that does CRUD operations over Exchange Rates
        /// </summary>
        public interface IExchangeRateService
        {
            /// <summary>
            /// Asynchronously gets exchange information for the two currency codes provided.
            /// </summary>
            /// <param name="fromCurrency">Currency code of the currency to exchange from.</param>
            /// <param name="toCurrency">Currency code of the currency to exchange to.</param>
            /// <returns>A<see cref = "ExchangeRateDto" /> containing the exchange rate information.</returns>
            /// <exception cref="ExchangeRateNotFoundException">Thrown when the specified currency codes are invalid or the exchange rate cannot be found.</exception>
            /// <exception cref="DbUpdateException">Thrown when an error occurs while updating the database.</exception>
            /// <exception cref="HttpRequestException">Thrown when an error occurs while communicating with the external AlphaVantage API.</exception>
            /// <exception cref="Exception">Thrown for any other unexpected exceptions.</exception>
            /// <remarks>
            /// This method first attempts to retrieve the exchange rate from the local database. 
            /// If the rate is not found, it fetches it from the external AlphaVantage API, stores it in the database, and then returns the result.
            /// </remarks>
            Task<ExchangeRateDto> GetExchangeRateAsync(string fromCurrency, string toCurrency);

            /// <summary>
            /// Asynchronously creates a new exchange rate.
            /// </summary>
            /// <param name="exchangeRateDto">The <seealso cref="ExchangeRateDto"/> containing the exchange rate data to be created.</param>
            /// <returns>
            /// Returns the created <seealso cref="ExchangeRateDto"/> if creation is successful.
            /// </returns>
            /// <exception cref="ExchangeRateAlreadyExistsException">Thrown when an exchange rate with the same FromCurrency and ToCurrency already exists.</exception>
            /// <exception cref="DbUpdateException">Thrown when a database update error occurs during creation.</exception>
            /// <exception cref="Exception">Thrown when an unexpected error occurs during creation.</exception>
            /// <remarks>
            /// This method attempts to create a new exchange rate in the database.
            /// It verifies that an exchange rate with the same FromCurrency and ToCurrency does not already exist before creating the new entry.
            /// The LastUpdate property of the created exchange rate is set to the current UTC time.
            /// </remarks>
            Task<ExchangeRateDto> CreateExchangeRateAsync(ExchangeRateDto exchangeRateDto);

            /// <summary>
            /// Asynchronously updates an exchange rate in the database.
            /// </summary>
            /// <param name="exchangeRateDto">The object containing the updated exchange rate information.</param>
            /// <returns>The updated exchange rate object.</returns>
            /// <exception cref="ExchangeRateNotFoundException">Thrown when the specified exchange rate is not found.</exception>
            /// <exception cref="DbUpdateException">Thrown when a database update error occurs.</exception>
            /// <exception cref="Exception">Thrown for any other unexpected exceptions during the update process.</exception>
            /// <remarks>
            /// This method retrieves an existing exchange rate from the database based on the FromCurrency and ToCurrency properties of the provided <seealso cref="ExchangeRateDto"/>.
            /// If the exchange rate is found, it updates the new exchange rate along wiht an updated LastUpdate timestamp, and saves the changes to the database.
            /// If the exchange rate is not found, an ExchangeRateNotFoundException is thrown.
            /// </remarks>
            Task<ExchangeRateDto> UpdateExchangeRateAsync(ExchangeRateDto exchangeRate);

            /// <summary>
            /// Asynchronously deletes an exchange rate based on the provided currencies.
            /// </summary>
            /// <param name="fromCurrency">The currency to exchange from.</param>
            /// <param name="toCurrency">The currency to exchange to.</param>
            /// <returns>
            /// Returns a task with the deleted exchange rate, if deletion is successful.
            /// </returns>
            /// <exception cref="ExchangeRateNotFoundException">Thrown when the specified exchange rate is not found.</exception>
            /// <exception cref="DbUpdateException">Thrown when a database update error occurs during deletion.</exception>
            /// <remarks>
            /// This method attempts to delete an exchange rate from the data store.
            /// It verifies the existence of the exchange rate before attempting deletion.
            /// </remarks>
            Task<ExchangeRateDto> DeleteExchangeRateAsync(string fromCurrency, string toCurrency);
        }
    }
}
