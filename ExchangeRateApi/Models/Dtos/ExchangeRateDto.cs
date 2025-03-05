using System.ComponentModel.DataAnnotations;
using ExchangeRateApi.Models.Attributes;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateApi.Models.Dtos
{
    public class ExchangeRateDto
    {
        [Required]
        [CurrencyCode]
        public string FromCurrency { get; set; }

        [Required]
        [CurrencyCode]
        public string ToCurrency { get; set; }

        [Required]
        [Precision(16, 8)]
        public decimal Rate { get; set; }

        [Required]
        [Precision(16, 8)]
        public decimal Bid { get; set; }

        [Required]
        [Precision(16, 8)]
        public decimal Ask { get; set; }
    }
}
