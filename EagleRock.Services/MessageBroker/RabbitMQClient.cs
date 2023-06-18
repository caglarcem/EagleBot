using RabbitMQ.Client;

namespace EagleRock.Services.MessageBroker
{
    public class RabbitMQClient
    {
        private readonly IConnection _connection;

        public RabbitMQClient(string host, int port, string username, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password
            };
            _connection = factory.CreateConnection();
        }

        public IModel CreateModel()
        {
            return _connection.CreateModel();
        }
    }
}
