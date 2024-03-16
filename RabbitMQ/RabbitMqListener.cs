using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using SmartApp.Models;

namespace SmartApp.RabbitMQ
{
    public class RabbitMqListener: BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel; 
        private readonly MessageHandlerRegistrator _messageHandlerRegistrator;
        private readonly string QueueName;
        private readonly string HostName;

        public RabbitMqListener(IOptions<RabbitMQSettings> rabbitMQSettingsAccessor, MessageHandlerRegistrator messageHandlerRegistrator)
        {
            _messageHandlerRegistrator = messageHandlerRegistrator;
            QueueName = rabbitMQSettingsAccessor.Value.QueueName;
            HostName = rabbitMQSettingsAccessor.Value.Host;

            var factory = new ConnectionFactory { HostName = HostName };
           
            _connection = factory.CreateConnection();
            
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, 
                                    durable: false, 
                                    exclusive: false, 
                                    autoDelete: false, 
                                    arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                await _messageHandlerRegistrator.Handle(ea.BasicProperties.Type, content);

                // _channel.BasicAck(ea.DeliveryTag, false); ошибка, что соединение закрыто
            };

            _channel.BasicConsume(QueueName, 
                                    true, 
                                    consumer);

            return Task.CompletedTask; 
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
