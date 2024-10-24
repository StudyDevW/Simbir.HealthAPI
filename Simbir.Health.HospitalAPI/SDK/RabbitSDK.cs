using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Simbir.Health.HospitalAPI.Model.Database.DTO.RabbitMQ;
using Simbir.Health.HospitalAPI.SDK.Services;
using System.Text;
using System.Text.Json;

namespace Simbir.Health.HospitalAPI.SDK
{
    public class RabbitSDK : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitSDK() {
            var rabbit = new ConnectionFactory()
            {
                HostName = "rabbitmq_broker",
                Port = 5672
            };

            _connection = rabbit.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessageObj(object message, string queue_name)
        {
            var serialized_message = JsonSerializer.Serialize(message);

            SetupSend(serialized_message, queue_name);
        }

        public void SetupSend(string message, string queue_name)
        {
            _channel.QueueDeclare(queue: queue_name,
                                 durable:    false,
                                 exclusive:  false,
                                 autoDelete: false,
                                 arguments:  null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                           routingKey: queue_name,
                           basicProperties: null,
                           body: body);
        }

        public async Task<Messages> GetMessages(string queue_name)
        {
            List<string> consumed_messages = new List<string>();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                consumed_messages.Add(content);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queue_name, false, consumer);

            return new Messages() { messages_consumed = consumed_messages };
        }


    }
}
