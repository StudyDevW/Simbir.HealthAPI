namespace Simbir.Health.HospitalAPI.SDK.Services
{
    public interface ICacheService
    {
        public T GetData<T>(string key);

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        public object RemoveData(string key);

        public void WriteKeyInStorage(int id_user, string type, string key, DateTime extime);

        public void DeleteKeyFromStorage(int id_user, string type);

        public bool CheckExistKeysStorage(int id_user, string type);

        public string? GetKeyFromStorage(int id_user, string type);
    }
}
