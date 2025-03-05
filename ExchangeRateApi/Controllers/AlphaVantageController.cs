using AutoMapper;
using ExchangeRateApi.Models.Attributes;
using ExchangeRateApi.Models.Dtos;
using ExchangeRateApi.Resources;
using ExchangeRateApi.Services.Interfaces;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ExchangeRateApi.Filters;
using Humanizer;


namespace ExchangeRateApi.Controllers
{
    /// <summary>
    /// Controller to access the external API https://www.alphavantage.co/
    /// </summary>
    [ValidateModelState]
    [Route("api/[controller]")]
    [ApiController]
    public class AlphaVantageController : ControllerBase
    {
        private readonly IAlphaVantageService _alphaVantageService;
        private readonly IMapper _mapper;

        public AlphaVantageController(IAlphaVantageService alphaVantageService, IMapper mapper)
        {
            _alphaVantageService = alphaVantageService;
            _mapper = mapper;
        }

        /// <summary>
        /// GET api/AlphaVantage/USD/EUR (e.g.)
        /// Gets exchange information for the two currency codes provided, 
        /// from the CURRENCY_EXCHANGE_RATE function of the AlphaVantage API.
        /// </summary>
        /// <param name="from">Currency code of the currency to exchange from.</param>
        /// <param name="to">Currency code of the currency to exchange to.</param>
        /// <returns>
        ///   <response code="201">Returns the exchange rate.</response>
        ///   <response code="400">If the exchange rate data is invalid.</response>
        ///   <response code="500">If an unexpected error occurs.</response>
        /// </returns>
        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<ResponseDto<ExchangeRateDto>>> GetExchangeRate([CurrencyCode] string from, [CurrencyCode] string to)
        {
            ResponseDto<ExchangeRateDto> responseDto = new();

            try
            {
                responseDto.Result = await _alphaVantageService.GetExchangeRateAsync(from, to);
                responseDto.Status = HttpStatusCode.OK;
                return Ok(responseDto);
            }
            catch (HttpRequestException ex)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> { ApiMessages.Error_AlphaVantage_HttpRequest.FormatWith(ex.Message) };
                return StatusCode(500, responseDto);
            }
            catch (JsonException ex)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> { ApiMessages.Error_AlphaVantage_ParsingJson.FormatWith(ex.Message) };
                return StatusCode(500, responseDto);
            }
            catch (Exception ex)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> { ApiMessages.Error_AlphaVantage_UnexpectedException.FormatWith(ex.Message) };
                return StatusCode(500, responseDto);
            }
        }
    }
}
