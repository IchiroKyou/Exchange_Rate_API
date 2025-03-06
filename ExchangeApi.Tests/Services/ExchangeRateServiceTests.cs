using AutoMapper;
using ExchangeRateApi.Data;
using ExchangeRateApi.Messaging;
using ExchangeRateApi.Messaging.Interfaces;
using ExchangeRateApi.Models;
using ExchangeRateApi.Models.Dtos;
using ExchangeRateApi.Services;
using ExchangeRateApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExchangeApi.Tests.Services
{
    public class ExchangeRateServiceTests : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture _databaseFixture;

        public ExchangeRateServiceTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetExchangeRateAsync_ExistingRateInDB_ReturnsRate()
        {
            #region Arrange

            string fromCurrency = "EUR";
            string toCurrency = "USD";
            ExchangeRate expectedExchangeRate = new ExchangeRate
            {
                ExchangeRateId = 1,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Ask = 1.01m,
                Bid = 0.06m,
                Rate = 1.05m,
                LastUpdate = DateTime.UtcNow
            };
            ExchangeRateDto expectedExchangeRateDto = new ExchangeRateDto
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Ask = 1.01m,
                Bid = 0.06m,
                Rate = 1.05m
            };

            ExchangeRateApiDbContext context = _databaseFixture.GetDbContext();
            context.Add(expectedExchangeRate);
            context.SaveChanges();


            // Mocks necessary for the ExchangeRateService
            ILogger<ExchangeRateService> logger = Mock.Of<ILogger<ExchangeRateService>>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ExchangeRateDto>(It.Is<ExchangeRate>(rate =>
                                                                            rate.ExchangeRateId == expectedExchangeRate.ExchangeRateId &&
                                                                            rate.FromCurrency == expectedExchangeRate.FromCurrency &&
                                                                            rate.ToCurrency == expectedExchangeRate.ToCurrency &&
                                                                            rate.Ask == expectedExchangeRate.Ask &&
                                                                            rate.Bid == expectedExchangeRate.Bid &&
                                                                            rate.Rate == expectedExchangeRate.Rate
                                                                        ))).Returns(expectedExchangeRateDto);

            Mock<IAlphaVantageService> alphaVantageServiceMock = MockHelpers.CreateAlphaVantageServiceMock(expectedExchangeRateDto);
            Mock<IMessageQueuePublisher<ExchangeRateDto>> exchangeRatePublisherMock = new Mock<IMessageQueuePublisher<ExchangeRateDto>>();
            exchangeRatePublisherMock.Setup(ep => ep.PublishMessageAsync(It.IsAny<ExchangeRateDto>())).Returns(Task.CompletedTask);

            #endregion

            #region Act

            IExchangeRateService exchangeRateService = new ExchangeRateService(logger, context, mapperMock.Object, alphaVantageServiceMock.Object, exchangeRatePublisherMock.Object);
            ExchangeRateDto exchangeRateServiceResponse = await exchangeRateService.GetExchangeRateAsync(fromCurrency, toCurrency);

            #endregion

            #region Assert

            // Assert
            alphaVantageServiceMock.Verify(a => a.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            Assert.NotNull(exchangeRateServiceResponse);
            Assert.IsType<ExchangeRateDto>(exchangeRateServiceResponse);
            Assert.Equal(expectedExchangeRateDto, exchangeRateServiceResponse);

            #endregion
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetExchangeRateAsync_NonExistingRateInDB_AccessesAlphaVantage_SavesToDB_ReturnsRate()
        {
            #region Arrange

            string fromCurrency = "EUR";
            string toCurrency = "USD";
            DateTime updateTime = DateTime.UtcNow;
            ExchangeRate expectedExchangeRate = new ExchangeRate
            {
                ExchangeRateId = 1,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Ask = 1.01m,
                Bid = 0.06m,
                Rate = 1.05m,
                LastUpdate = updateTime
            };
            ExchangeRateDto expectedExchangeRateDto = new ExchangeRateDto
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Ask = 1.01m,
                Bid = 0.06m,
                Rate = 1.05m
            };

            // Mocks necessary for the ExchangeRateService
            ILogger<ExchangeRateService> logger = Mock.Of<ILogger<ExchangeRateService>>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ExchangeRate>(It.Is<ExchangeRateDto>(rateDto =>
                                                                            rateDto.FromCurrency == expectedExchangeRate.FromCurrency &&
                                                                            rateDto.ToCurrency == expectedExchangeRate.ToCurrency &&
                                                                            rateDto.Ask == expectedExchangeRate.Ask &&
                                                                            rateDto.Bid == expectedExchangeRate.Bid &&
                                                                            rateDto.Rate == expectedExchangeRate.Rate
                                                                        ))).Returns(expectedExchangeRate);
            mapperMock.Setup(m => m.Map<ExchangeRateDto>(It.Is<ExchangeRate>(rate =>
                                                                            rate.ExchangeRateId == expectedExchangeRate.ExchangeRateId &&
                                                                            rate.FromCurrency == expectedExchangeRate.FromCurrency &&
                                                                            rate.ToCurrency == expectedExchangeRate.ToCurrency &&
                                                                            rate.Ask == expectedExchangeRate.Ask &&
                                                                            rate.Bid == expectedExchangeRate.Bid &&
                                                                            rate.Rate == expectedExchangeRate.Rate
                                                                        ))).Returns(expectedExchangeRateDto);

            Mock<IAlphaVantageService> alphaVantageServiceMock = MockHelpers.CreateAlphaVantageServiceMock(expectedExchangeRateDto);
            Mock<IMessageQueuePublisher<ExchangeRateDto>> exchangeRatePublisherMock = new Mock<IMessageQueuePublisher<ExchangeRateDto>>();
            exchangeRatePublisherMock.Setup(ep => ep.PublishMessageAsync(It.IsAny<ExchangeRateDto>())).Returns(Task.CompletedTask);

            #endregion

            #region Act

            ExchangeRateApiDbContext context = _databaseFixture.GetDbContext();

            IExchangeRateService exchangeRateService = new ExchangeRateService(logger, context, mapperMock.Object, alphaVantageServiceMock.Object, exchangeRatePublisherMock.Object);
            ExchangeRateDto exchangeRateServiceResponse = await exchangeRateService.GetExchangeRateAsync(fromCurrency, toCurrency);

            #endregion

            #region Assert

            alphaVantageServiceMock.Verify(a => a.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            ExchangeRate exchangeRateFromDb = context.ExchangeRates.FirstOrDefault();


            // Checks service response
            Assert.NotNull(exchangeRateServiceResponse);
            Assert.IsType<ExchangeRateDto>(exchangeRateServiceResponse);

            // Checks returned exchange rate
            Assert.Equal(expectedExchangeRateDto, exchangeRateServiceResponse);

            // Checks saved DB exchange rate
            Assert.NotNull(exchangeRateFromDb);
            Assert.Equal(expectedExchangeRate, exchangeRateFromDb);

            #endregion
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task PostExchangeRateAsync_NewRate_ReturnsCreatedRate()
        {
            #region Arrange

            ExchangeRateDto newExchangeRateDto = new ExchangeRateDto
            {
                FromCurrency = "USD",
                ToCurrency = "GBP",
                Rate = 0.75m,
                Bid = 1.2m,
                Ask = 3.5m
            };
            ExchangeRate expectedExchangeRate = new ExchangeRate
            {
                FromCurrency = "USD",
                ToCurrency = "GBP",
                Rate = 0.75m,
                Bid = 1.2m,
                Ask = 3.5m,
            };

            ILogger<ExchangeRateService> logger = Mock.Of<ILogger<ExchangeRateService>>();
            Mock<IMapper> mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ExchangeRate>(It.Is<ExchangeRateDto>(rateDto =>
                                                                rateDto.FromCurrency == newExchangeRateDto.FromCurrency &&
                                                                rateDto.ToCurrency == newExchangeRateDto.ToCurrency &&
                                                                rateDto.Ask == newExchangeRateDto.Ask &&
                                                                rateDto.Bid == newExchangeRateDto.Bid &&
                                                                rateDto.Rate == newExchangeRateDto.Rate
                                                            ))).Returns(expectedExchangeRate);
            IAlphaVantageService alphaVantageService = Mock.Of<IAlphaVantageService>();
            Mock<IMessageQueuePublisher<ExchangeRateDto>> exchangeRatePublisherMock = new Mock<IMessageQueuePublisher<ExchangeRateDto>>();
            exchangeRatePublisherMock.Setup(ep => ep.PublishMessageAsync(It.IsAny<ExchangeRateDto>())).Returns(Task.CompletedTask);

            ExchangeRateApiDbContext context = _databaseFixture.GetDbContext();

            ExchangeRateService exchangeRateService = new ExchangeRateService(logger, context, mapperMock.Object, alphaVantageService, exchangeRatePublisherMock.Object);

            #endregion

            #region Act

            ExchangeRateDto exchangeRateServiceResponse = await exchangeRateService.CreateExchangeRateAsync(newExchangeRateDto);

            ExchangeRate? savedExchangeRate = context.ExchangeRates.FirstOrDefault();

            #endregion

            #region Assert

            Assert.NotNull(exchangeRateServiceResponse);
            Assert.Equal(newExchangeRateDto, exchangeRateServiceResponse);

            Assert.NotNull(savedExchangeRate);
            Assert.Equal(expectedExchangeRate.FromCurrency, savedExchangeRate.FromCurrency);
            Assert.Equal(expectedExchangeRate.ToCurrency, savedExchangeRate.ToCurrency);
            Assert.Equal(expectedExchangeRate.Bid, savedExchangeRate.Bid);
            Assert.Equal(expectedExchangeRate.Ask, savedExchangeRate.Ask);

            #endregion
        }

        public static class MockHelpers
        {
            public static Mock<IAlphaVantageService> CreateAlphaVantageServiceMock(ExchangeRateDto expectedExchangeRateDto)
            {
                ILogger<AlphaVantageService> logger = Mock.Of<ILogger<AlphaVantageService>>();
                IConfiguration configuration = Mock.Of<IConfiguration>();
                IMapper mapper = Mock.Of<IMapper>();
                IHttpClientFactory httpClientFactory = Mock.Of<IHttpClientFactory>();

                //var alphaVantageServiceMock = new Mock<AlphaVantageService>(logger, configuration, mapper, httpClientFactory);
                Mock<IAlphaVantageService> alphaVantageServiceMock = new Mock<IAlphaVantageService>();
                alphaVantageServiceMock.Setup(a => a.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedExchangeRateDto);

                return alphaVantageServiceMock;
            }
        }
    }

    public class DatabaseFixture : IDisposable
    {
        public DbContextOptions<ExchangeRateApiDbContext> Options { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        private ExchangeRateApiDbContext _context;

        public ExchangeRateApiDbContext GetDbContext()
        {
            if (_context is not null)
            {
                _context.Dispose();
            }

            //// For the in-memory database context
            Options = new DbContextOptionsBuilder<ExchangeRateApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            Configuration = new ConfigurationBuilder().Build();
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
            _context = new ExchangeRateApiDbContext(Options, Configuration, LoggerFactory);

            return _context;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
