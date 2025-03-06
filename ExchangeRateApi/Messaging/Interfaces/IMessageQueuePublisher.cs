namespace ExchangeRateApi.Messaging.Interfaces
{
    public interface IMessageQueuePublisher<T> where T : class
    {
        Task PublishMessageAsync(T message);
    }
}
