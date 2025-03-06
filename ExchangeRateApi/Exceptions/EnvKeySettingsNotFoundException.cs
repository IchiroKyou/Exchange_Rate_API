using ExchangeRateApi.Resources;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    public class EnvKeySettingsNotFoundException : Exception
    {
        public EnvKeySettingsNotFoundException(string envKeyName) 
            : base(ApiMessages.Error_EnvKeyNotFound.FormatWith(envKeyName)) 
        { }
    }
}
