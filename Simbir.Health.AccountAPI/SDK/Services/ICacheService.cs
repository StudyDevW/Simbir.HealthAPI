namespace Simbir.Health.AccountAPI.SDK.Services
{
    public interface ICacheService
    {
        public T GetData<T>(string key);

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        public object RemoveData(string key);

        public void WriteKeyInStorage(string userName, string type, string key, DateTime extime);

        public void DeleteKeyFromStorage(string userName, string type);

        public bool CheckExistKeysStorage(string userName, string type);

        public string? GetKeyFromStorage(string userName, string type);
    }
}
