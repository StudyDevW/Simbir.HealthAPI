using Simbir.Health.AccountAPI.SDK.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Simbir.Health.AccountAPI.SDK
{
    public class CacheSDK : ICacheService
    {
        private IDatabase _cacheDb;

        public CacheSDK()
        {
            var redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { "redis_cache:6379" },
                    Password = "root",
                    AbortOnConnectFail = false
                });

            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);

            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);


            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);

            if (_exist)
                return _cacheDb.KeyDelete(key);


            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }

        public void WriteKeyInStorage(int id_user, string type, string key, DateTime extime)
        {
            SetData($"{type}_storage_{id_user}", key, extime);
        }

        public void DeleteKeyFromStorage(int id_user, string type)
        {
            RemoveData($"{type}_storage_{id_user}");
        } 

        public bool CheckExistKeysStorage(int id_user, string type)
        {
            var cache_data = GetData<string>($"{type}_storage_{id_user}");

            if (cache_data != null)
                return true;

            return false;
        }

        public string? GetKeyFromStorage(int id_user, string type)
        {
            var cache_data = GetData<string>($"{type}_storage_{id_user}");

            return cache_data;
        }
    }
}
