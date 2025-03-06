using ExchangeRateApi.Models.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ExchangeRateApi.Models
{
    /// <summary>
    /// Entity for representing an exchange rate between two currencies.
    /// </summary>
    [Index(nameof(FromCurrency), nameof(ToCurrency))] // To ensure that there are no repeated (From, To) database entries
    public class ExchangeRate
    {
        /// <summary>
        /// Primary key for the exchange rate.
        /// </summary>
        [Key]
        public int ExchangeRateId { get; set; }

        /// <summary>
        /// The currency code of the currency to be exchanged from.
        /// </summary>
        [Required]
        [CurrencyCode]
        public string FromCurrency { get; set; }

        /// <summary>
        /// The currency code of the currency to be exchanged to.
        /// </summary>
        [Required]
        [CurrencyCode]
        public string ToCurrency { get; set; }

        /// <summary>
        /// Rate of exchange between <seealso cref="FromCurrency"/> and <seealso cref="ToCurrency"/>.
        /// </summary>
        [Required]
        [Precision(16, 8)]
        public decimal Rate { get; set; }

        /// <summary>
        /// Bid value of the exchange between <seealso cref="FromCurrency"/> and <seealso cref="ToCurrency"/>.
        /// </summary>
        [Required]
        [Precision(16, 8)]
        public decimal Bid { get; set; }

        /// <summary>
        /// Ask value of the exchange between <seealso cref="FromCurrency"/> and <seealso cref="ToCurrency"/>.
        /// </summary>
        [Required]
        [Precision(16, 8)]
        public decimal Ask { get; set; }

        /// <summary>
        /// Last time this exchange rate was updated.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastUpdate { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
