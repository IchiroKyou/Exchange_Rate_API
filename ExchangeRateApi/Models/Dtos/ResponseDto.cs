using System.Net;

namespace ExchangeRateApi.Models.Dtos
{
    /// <summary>
    /// A generic response for encapsulating API responses.
    /// </summary>
    /// <typeparam name="T">The type of the result data within the response.</typeparam>    
    public class ResponseDto<T>
    {
        public HttpStatusCode Status { get; set; }
        public T Result { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
