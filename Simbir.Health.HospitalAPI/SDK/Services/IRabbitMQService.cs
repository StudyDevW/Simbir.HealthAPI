using Simbir.Health.HospitalAPI.Model.Database.DTO.RabbitMQ;

namespace Simbir.Health.HospitalAPI.SDK.Services
{
    public interface IRabbitMQService
    {
        public void SetupSend(string message, string queue_name);

        public void SendMessageObj(object message, string queue_name);

        public Task<Messages> GetMessages(string queue_name);
    }
}
