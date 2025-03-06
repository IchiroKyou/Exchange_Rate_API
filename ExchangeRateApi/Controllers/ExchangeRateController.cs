using AutoMapper;
using ExchangeRateApi.Exceptions;
using ExchangeRateApi.Models.Attributes;
using ExchangeRateApi.Models.Dtos;
using ExchangeRateApi.Models;
using ExchangeRateApi.Resources;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeRateApi.Services.Interfaces;
using ExchangeRateApi.Data;
using Humanizer;


namespace ExchangeRateApi.Controllers
{
    /// <summary>
    /// Controller to perform CRUD operations over exchange rates.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<ExchangeRateController> _logger;
        private readonly ExchangeRateApiDbContext _context;
        private readonly IMapper _mapper;

        public ExchangeRateController(IExchangeRateService exchangeRateService, ILogger<ExchangeRateController> logger, ExchangeRateApiDbContext context, IMapper mapper)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// GET: api/ExchangeRate/EUR/USD (e.g.)
        /// Gets exchange information for the two currency codes provided.
        /// </summary>
        /// <param name="from">Currency code of the currency to exchange from.</param>
        /// <param name="to">Currency code of the currency to exchange to.</param>
        /// <returns>
        ///   <response code="201">Currency exchange information for the two currencies provided.</response>
        ///   <response code="400">If any of the currency codes are invalid.</response>
        ///   <response code="500">If an unexpected error occurs.</response>
        /// </returns>
        [HttpGet("{from}/{to}")]
        public async Task<ActionResult<ResponseDto<ExchangeRateDto>>> GetExchangeRate([CurrencyCode] string from, [CurrencyCode] string to)
        {
            ResponseDto<ExchangeRateDto> responseDto = new();

            try
            {
                responseDto.Status = HttpStatusCode.Created;
                responseDto.Result = await _exchangeRateService.GetExchangeRateAsync(from, to);
                return Ok(responseDto);
            }
            catch (DbUpdateException)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> { ApiMessages.Error_ExchangeRateController_Get_DefaultError };
                return StatusCode(500, responseDto);
            }
            catch (HttpRequestException)
            {
                responseDto.Status = HttpStatusCode.NotFound;
                responseDto.Errors = new List<string> { ApiMessages.Error_ExchangeRatesController_RateNotFound.FormatWith(from, to) };
                return NotFound(responseDto);
            }
            catch (Exception)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> { ApiMessages.Error_ExchangeRateController_Get_DefaultError };
                return StatusCode(500, responseDto);
            }
        }

        /// <summary>
        /// POST: api/ExchangeRate
        /// Adds a new exchange rate to the database.
        /// </summary>
        /// <param name="exchangeRateDto">Exchange rate to add.</param>
        /// <returns>
        ///   <response code="201">Returns the newly created exchange rate.</response>
        ///   <response code="400">If the exchange rate data is invalid.</response>
        ///   <response code="500">If an unexpected error occurs.</response>
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<ExchangeRate>> PostExchangeRate(ExchangeRateDto exchangeRateDto)
        {
            ResponseDto<ExchangeRateDto> responseDto = new();

            try
            {
                responseDto.Result = await _exchangeRateService.CreateExchangeRateAsync(exchangeRateDto);
                responseDto.Status = HttpStatusCode.OK;
                return CreatedAtAction(nameof(PostExchangeRate), responseDto);
            }
            catch (DbUpdateException)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Post_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return StatusCode(500, responseDto);
            }
            catch (Exception)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Post_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return StatusCode(500, responseDto);
            }
        }

        /// <summary>
        /// PUT: api/ExchangeRate
        /// Updates an existing exchange rate based on the provided <seealso cref="ExchangeRateDto"/>.
        /// </summary>
        /// <param name="exchangeRateDto">The ExchangeRateDto containing the updated exchange rate information.</param>
        /// <returns>
        ///   <para>200 OK: If the exchange rate is successfully updated, returns the updated ExchangeRateDto within a ResponseDto.</para>
        ///   <para>404 Not Found: If the exchange rate with the specified currency codes is not found, returns a NotFound response with an error message in a ResponseDto.</para>
        ///   <para>500 Internal Server Error: If a concurrency error occurs during the update, or if any other unexpected exception is thrown, returns an Internal Server Error response with an error message.</para>
        /// </returns>
        /// <remarks>
        /// This endpoint updates an exchange rate by matching the provided 'fromCurrency' and 'toCurrency' in the database.
        /// The LastUpdate property of the ExchangeRate entity is updated to the current UTC time.
        /// </remarks>
        /// <response code="200">Returns the updated ExchangeRateDto.</response>
        /// <response code="404">Returns a NotFound response if the exchange rate is not found.</response>
        /// <response code="500">Returns an Internal Server Error response if an error occurs during the update.</response>
        [HttpPut]
        public async Task<ActionResult<ResponseDto<ExchangeRateDto>>> PutExchangeRate(ExchangeRateDto exchangeRateDto)
        {
            ResponseDto<ExchangeRateDto> responseDto = new();

            try
            {
                responseDto.Result = await _exchangeRateService.UpdateExchangeRateAsync(exchangeRateDto);
                responseDto.Status = HttpStatusCode.OK;
                return Ok(responseDto);
            }
            catch (ExchangeRateNotFoundException)
            {
                responseDto.Status = HttpStatusCode.NotFound;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRateController_Put_ExchangeRateNotFound
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return NotFound(responseDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Put_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return StatusCode(500, responseDto);
            }
            catch (DbUpdateException)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Put_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return StatusCode(500, responseDto);
            }
            catch (Exception)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Put_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency)
                };
                return StatusCode(500, responseDto);
            }
        }

        /// <summary>
        /// DELETE: api/ExchangeRate/From/To
        /// Deletes an exchange rate based on the provided currencies.
        /// </summary>
        /// <param name="fromCurrency">The currency to exchange from.</param>
        /// <param name="toCurrency">The currency to exchange to.</param>
        /// <returns>
        ///   - NoContent (204): If the exchange rate is successfully deleted.
        ///   - NotFound (404): If the exchange rate with the specified currencies is not found.
        ///   - InternalServerError (500): If an unexpected error or a database update error occurs.
        /// </returns>
        /// <response code="204">No Content. The exchange rate was successfully deleted.</response>
        /// <response code="404">Not Found. The exchange rate with the specified currencies was not found.</response>
        /// <response code="500">Internal Server Error. An unexpected error occurred during deletion.</response>
        [HttpDelete("{from}/{to}")]
        public async Task<IActionResult> DeleteExchangeRate([CurrencyCode] string fromCurrency, [CurrencyCode] string toCurrency)
        {
            ResponseDto<ExchangeRateDto> responseDto = new();

            try
            {
                await _exchangeRateService.DeleteExchangeRateAsync(fromCurrency, toCurrency);
                return NoContent();
            }
            catch (ExchangeRateNotFoundException)
            {
                responseDto.Status = HttpStatusCode.NotFound;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRateController_Delete_ExchangeRateNotFound
                            .FormatWith(fromCurrency, toCurrency)
                };
                return NotFound(responseDto);
            }
            catch (DbUpdateException)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Delete_DefaultError
                            .FormatWith(fromCurrency, toCurrency)
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, responseDto);
            }
            catch (Exception)
            {
                responseDto.Status = HttpStatusCode.InternalServerError;
                responseDto.Errors = new List<string> {
                    ApiMessages.Error_ExchangeRatesController_Delete_DefaultError
                            .FormatWith(fromCurrency, toCurrency)
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, responseDto);
            }
        }
    }
}
