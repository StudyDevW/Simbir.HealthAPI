namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface IRabbitListenerService
    {
        public void Listen();
        public void Close();
    }
}
