using ExchangeRateApi.Resources;
using Humanizer;

namespace ExchangeRateApi.Exceptions
{
    public class EnvVariableNotFoundException : Exception
    {
        public EnvVariableNotFoundException(string envVariableName) 
            : base(ApiMessages.Error_EnvKeyNotFound.FormatWith(envVariableName)) 
        { }
    }
}
