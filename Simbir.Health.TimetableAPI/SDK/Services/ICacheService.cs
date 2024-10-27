namespace Simbir.Health.TimetableAPI.SDK.Services
{
    public interface ICacheService
    {
        public T GetData<T>(string key);

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        public object RemoveData(string key);
    }
}
