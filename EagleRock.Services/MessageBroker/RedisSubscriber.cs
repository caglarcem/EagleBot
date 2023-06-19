using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

namespace EagleRock.Services.MessageBroker
{
    public class RedisSubscriber
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly ISubscriber _subscriber;
        private readonly SemaphoreSlim _semaphore;

        public RedisSubscriber(RabbitMQClient rabbitMQClient, string connectionString, string channelName)
        {
            _rabbitMQClient = rabbitMQClient;
            _semaphore = new SemaphoreSlim(20);

            var connection = ConnectionMultiplexer.Connect(connectionString);
            _subscriber = connection.GetSubscriber();
            _subscriber.Subscribe(channelName, OnMessageReceived);
        }

        private void OnMessageReceived(RedisChannel channel, RedisValue message)
        {
            _semaphore.Wait();

            try
            {
                using (var model = _rabbitMQClient.CreateModel())
                {
                    model.ExchangeDeclare(exchange: "events_exchange", type: ExchangeType.Topic, true);
                    var body = Encoding.UTF8.GetBytes(message.ToString());
                    model.BasicPublish(exchange: "events_exchange", routingKey: "event.message", basicProperties: null, body: body);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
