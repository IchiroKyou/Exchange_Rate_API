using System.Text;
using System.Text.Json;
using ExchangeRateApi.Exceptions;
using ExchangeRateApi.Messaging.Interfaces;
using ExchangeRateApi.Models.Dtos;
using RabbitMQ.Client;

namespace ExchangeRateApi.Messaging
{
    public class ExchangeRatePublisher : IMessageQueuePublisher<ExchangeRateDto>
    {
        private string _mqHostName; // "localhost";

        private readonly IConfiguration _configuration;
        private readonly ILogger<ExchangeRatePublisher> _logger;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string MQ_QUEUE_NAME = "exchangeRatesQueue";
        private const string ENV_SETTINGS_KEY = "EnvVarKeys:MqHostName";

        public ExchangeRatePublisher(IConfiguration configuration, ILogger<ExchangeRatePublisher> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task PublishMessageAsync(ExchangeRateDto exchangeRateDto)
        {
            try
            {
                await CreateConnectionAsync();

                // Serialize the message (example: using JSON)
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(exchangeRateDto));

                // Publish the message to the queue
                await _channel.BasicPublishAsync(exchange: "", routingKey: MQ_QUEUE_NAME, body: body);
            }
            catch(EnvKeySettingsNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            catch(EnvVariableNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "");
            }
        }

        private async Task CreateConnectionAsync()
        {
            string mqHostNameEnvKey = _configuration.GetValue<string>(ENV_SETTINGS_KEY) ?? string.Empty;

            // Check for the absence of the environment variable key settings in the appsettings.json
            if (string.IsNullOrEmpty(mqHostNameEnvKey))
            {
                throw new EnvKeySettingsNotFoundException(ENV_SETTINGS_KEY);
            }

            // Retrieves the mq hostname from an env variable (different for each env)
            _mqHostName = Environment.GetEnvironmentVariable(mqHostNameEnvKey) ?? string.Empty;

            // Check if the hostname could not be fetched from the environment variable
            if (string.IsNullOrEmpty(_mqHostName))
            {
                throw new EnvVariableNotFoundException(mqHostNameEnvKey);
            }

            if (_channel is null)
            {
                var factory = new ConnectionFactory() { HostName = _mqHostName };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(
                    queue: MQ_QUEUE_NAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
        }
    }
}
