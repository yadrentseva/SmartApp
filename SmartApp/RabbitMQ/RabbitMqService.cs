using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SmartApp.Models;
using System.Text;
using System.Text.Json;

namespace SmartApp.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly string? HostName;
        private readonly string? QueueName;
        private readonly string? UserName;
        private readonly string? Password;

        IConnection _connection;

        public RabbitMqService(IOptions<RabbitMQSettings> rabbitMQSettingsAccessor)
        {
            HostName = rabbitMQSettingsAccessor.Value.Host;
            QueueName = rabbitMQSettingsAccessor.Value.QueueName;
            UserName = rabbitMQSettingsAccessor.Value.Username;
            Password = rabbitMQSettingsAccessor.Value.Password;

            CreateConnection();
        }

        public void SendMessage(string typeMessage, object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(typeMessage, message);
        }

        public void SendMessage(string typeMessage, string message = "")
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: QueueName,
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Type = typeMessage;

                    channel.BasicPublish(exchange: "",
                                   routingKey: QueueName,
                                   basicProperties: properties,
                                   body: body);
                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = HostName,
                    UserName = UserName,
                    Password = Password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }

    }
}
