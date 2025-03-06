using AutoMapper;
using ExchangeRateApi.Data;
using ExchangeRateApi.Exceptions;
using ExchangeRateApi.Models.Dtos;
using ExchangeRateApi.Models;
using ExchangeRateApi.Resources;
using ExchangeRateApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Humanizer;

namespace ExchangeRateApi.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly ExchangeRateApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlphaVantageService _alphaVantageService;

        private const int CONCURRENCY_MAX_RETRIES = 5;

        public ExchangeRateService(ILogger<ExchangeRateService> logger, ExchangeRateApiDbContext context, IMapper mapper, IAlphaVantageService alphaVantageService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _alphaVantageService = alphaVantageService;
        }

        public async Task<ExchangeRateDto> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var exchangeRate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(rate => rate.FromCurrency == fromCurrency && rate.ToCurrency == toCurrency);

                // If the database does not have the exchange rate fetch from external API, and save to database
                if (exchangeRate is null)
                {
                    var exchangeRateDto = await _alphaVantageService.GetExchangeRateAsync(fromCurrency, toCurrency);

                    exchangeRate = _mapper.Map<ExchangeRate>(exchangeRateDto);
                    _context.ExchangeRates.Add(exchangeRate);
                    await _context.SaveChangesAsync();

                    // TODO: Fire event in messaging queue system (e.g. RabbitMQ)
                    return exchangeRateDto;
                }

                return _mapper.Map<ExchangeRateDto>(exchangeRate);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRateController_DatabaseUpdate.FormatWith(ex.Message));
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Error_Contacting_AlphaVantage.FormatWith(ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRateController_Get_DefaultError.FormatWith(ex.Message));
                throw;
            }
        }

        public async Task<ExchangeRateDto> CreateExchangeRateAsync(ExchangeRateDto exchangeRateDto)
        {
            try
            {
                var existingExchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(
                    rate => rate.FromCurrency == exchangeRateDto.FromCurrency && rate.ToCurrency == exchangeRateDto.ToCurrency);

                // Check for duplicates
                if (existingExchangeRate != null)
                {
                    throw new ExchangeRateAlreadyExistsException(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency);
                }

                var exchangeRate = _mapper.Map<ExchangeRate>(exchangeRateDto);

                _context.ExchangeRates.Add(exchangeRate);
                await _context.SaveChangesAsync();

                return exchangeRateDto;
            }
            catch (ExchangeRateAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, ApiMessages.Error_ExchangeRateAlreadyExists
                        .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Post_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Post_DefaultError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));
                throw;
            }
        }

        public async Task<ExchangeRateDto> UpdateExchangeRateAsync(ExchangeRateDto exchangeRateDto)
        {
            int concurrencyRetryCount = 0;

            try
            {
                // Here we have an added roundtrip to the database because we are not providing any primary key.
                ExchangeRate? exchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(
                    rate => rate.FromCurrency == exchangeRateDto.FromCurrency && rate.ToCurrency == exchangeRateDto.ToCurrency);

                if (exchangeRate is null)
                {
                    _logger.LogWarning(ApiMessages.Error_ExchangeRateController_Put_ExchangeRateNotFound
                        .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));
                    throw new ExchangeRateNotFoundException(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency);
                }

                _mapper.Map(exchangeRateDto, exchangeRate);
                await _context.SaveChangesAsync();

                return exchangeRateDto;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Retries CONCURRENCY_MAX_RETRIES times
                if (concurrencyRetryCount++ > CONCURRENCY_MAX_RETRIES)
                {
                    _logger.LogError(ApiMessages.Error_ExchangeRatesController_Put_RetryLimitExceeded
                        .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency, CONCURRENCY_MAX_RETRIES));
                    throw;
                }

                _logger.LogWarning(ex, ApiMessages.Error_ExchangeRatesController_Put_ConcurrencyError
                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));

                var entry = ex.Entries.Single();
                await entry.ReloadAsync();

                _mapper.Map(exchangeRateDto, entry.Entity);
                return exchangeRateDto;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Put_DefaultError
                                    .FormatWith(exchangeRateDto.FromCurrency, exchangeRateDto.ToCurrency));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Put_DefaultError);
                throw;
            }
        }

        public async Task<ExchangeRateDto> DeleteExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                // Here we have an added roundtrip to the database because we are not providing any primary key.
                ExchangeRate? exchangeRate = await _context.ExchangeRates.FirstOrDefaultAsync(
                    rate => rate.FromCurrency == fromCurrency && rate.ToCurrency == toCurrency);

                if (exchangeRate == null)
                {
                    throw new ExchangeRateNotFoundException(fromCurrency, toCurrency);
                }

                _context.ExchangeRates.Remove(exchangeRate);
                await _context.SaveChangesAsync();

                return _mapper.Map<ExchangeRateDto>(exchangeRate);
            }
            catch (ExchangeRateNotFoundException ex)
            {
                _logger.LogWarning(ex, ApiMessages.Error_ExchangeRateController_Delete_ExchangeRateNotFound
                            .FormatWith(fromCurrency, toCurrency));
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Delete_DefaultError
                            .FormatWith(fromCurrency, toCurrency));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.Error_ExchangeRatesController_Delete_DefaultError
                            .FormatWith(fromCurrency, toCurrency));
                throw;
            }
        }
    }
}
