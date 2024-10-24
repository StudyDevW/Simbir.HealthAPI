using Simbir.Health.AccountAPI.Model.Database.DTO.RabbitMQ;

namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IRabbitMQService
    {
        public void SetupSend(string message, string queue_name);

        public void SendMessageObj(object message, string queue_name);

        public Task<Messages> GetMessages(string queue_name);
    }
}
