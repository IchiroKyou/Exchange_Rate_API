using System.ComponentModel.DataAnnotations;
using ExchangeRateApi.Resources;

namespace ExchangeRateApi.Models.Attributes
{
    public class CurrencyCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string currencyCode)
            {
                if (!string.IsNullOrEmpty(currencyCode) && currencyCode.Length == 3 && currencyCode.All(char.IsLetter))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ApiMessages.Validation_CurrencyCode_InvalidCodeFormat);
        }
    }
}
