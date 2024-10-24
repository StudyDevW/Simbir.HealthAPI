using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Simbir.Health.AccountAPI.Model.Database.DTO.RabbitMQ;
using Simbir.Health.AccountAPI.SDK.Services;
using System.Text;
using System.Threading.Channels;

namespace Simbir.Health.AccountAPI.SDK
{
    public class RabbitListener : IRabbitListenerService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IRabbitMQService _rabbitmq;
        private readonly IJwtService _jwt;
        public RabbitListener(IRabbitMQService rabbitService, IJwtService jwtService)
        {
            var rabbit = new ConnectionFactory()
            {
                HostName = "rabbitmq_broker",
                Port = 5672
            };

            _connection = rabbit.CreateConnection();
            _channel = _connection.CreateModel();

            _rabbitmq = rabbitService;
            _jwt = jwtService;
        }

        public void Listen()
        {
            GetMessageFromAction("validation_queue");
        }

        private void GetMessageFromAction(string queue_name)
        {
            _channel.QueueDeclare(queue: queue_name,
                       durable: false,
                       exclusive: false,
                       autoDelete: false,
                       arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, eventArgs) =>
            {
                var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                MessageOperation(queue_name, content).Start();

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queue_name, false, consumer);
        }

        private async Task MessageOperation(string queue_name, string content)
        {
            if (queue_name == "validation_queue")
            {
                await ValidationRequest(content);
            }
        }

        private async Task ValidationRequest(string message)
        {
    
            Rabbit_Validation rabbitNotValid = new Rabbit_Validation()
            {
                token = message,
                status = "not_valid"
            };

            Rabbit_Validation rabbitValid = new Rabbit_Validation()
            {
                token = message,
                status = "valid"
            };

            var validation = await _jwt.AccessTokenValidation(message);

            if (validation.TokenHasError())
            {
                _rabbitmq.SendMessageObj(rabbitNotValid, "validation_queue_response");
            }
            else if (validation.TokenHasSuccess())
            {
                _rabbitmq.SendMessageObj(rabbitValid, "validation_queue_response");
            }
        }

        public void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
