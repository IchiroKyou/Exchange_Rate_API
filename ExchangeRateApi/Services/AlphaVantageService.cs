using AutoMapper;
using System.Text.Json;
using ExchangeRateApi.Models.Dtos;
using ExchangeRateApi.Resources;
using ExchangeRateApi.Services.Interfaces;
using ExchangeRateApi.Exceptions;
using Humanizer;

namespace ExchangeRateApi.Services
{
    public class AlphaVantageService : IAlphaVantageService
    {
        private readonly ILogger<AlphaVantageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public AlphaVantageService(ILogger<AlphaVantageService> logger, IConfiguration configuration, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ExchangeRateDto> GetExchangeRateAsync(string from, string to)
        {
            string settingsKey = "EnvVarKeys:AlphavantageApi";
            var apiEnvKey = _configuration.GetValue<string>(settingsKey);

            try
            {
                // If the environment variable key is not in the appsettings.json return error code
                if (string.IsNullOrEmpty(apiEnvKey))
                {
                    throw new AlphaVantageApiKeySettingsNotFoundException(settingsKey);
                }

                var apiKey = Environment.GetEnvironmentVariable(apiEnvKey);

                // If API key is not set in the environment variable return error code
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new AlphaVantageApiKeyNotFoundException();
                }

                string apiUrl = $"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={from}&to_currency={to}&apikey={apiKey}";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode(); // Throws exception if status code is not success

                string responseBody = await response.Content.ReadAsStringAsync();
                var alphaVantageResponseDto = JsonSerializer.Deserialize<AlphaVantageResponseDto>(responseBody);

                // Check if for some reason the RealtimeCurrencyExchangeRate inside the response couldn't be serialized
                if (alphaVantageResponseDto?.RealtimeCurrencyExchangeRate is null)
                {
                    throw new ExchangeRateNotFoundException(from, to);
                }

                var exchangeRateDto = _mapper.Map<ExchangeRateDto>(alphaVantageResponseDto);
                // TODO: IMPROVEMENTS: Here, the exchange rate will not have the timestamp that comes from the external API. Is it necessary?

                return exchangeRateDto;
            }
            catch (AlphaVantageApiKeySettingsNotFoundException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_EnvKeyNotFound.FormatWith(settingsKey));
                throw;
            }
            catch (AlphaVantageApiKeyNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (ExchangeRateNotFoundException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_AlphaVantage_NotFound.FormatWith(from, to));
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_AlphaVantage_HttpRequest.FormatWith(ex.Message));
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, ApiMessages.Error_AlphaVantage_ParsingJson.FormatWith(ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.Error_AlphaVantage_UnexpectedException.FormatWith(ex.Message));
                throw;
            }
        }
    }
}
