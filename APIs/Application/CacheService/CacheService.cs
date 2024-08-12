using Application.InterfaceService;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _database;
        public CacheService(IDatabase database)
        {
            _database = database;
        }

        public T GetData<T>(string key)
        {
            var value = _database.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _database.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _database.KeyDelete(key);
            }
            return false;
        }
        public object UpdateData(string key, object value)
        {
            bool _isKeyExist = _database.KeyExists(key);
            if (_isKeyExist == true)
            {
                SetData<object>(key, value, DateTime.Now.AddMinutes(20));
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset? expirationTime=null)
        {
            TimeSpan? expiryTime = expirationTime.HasValue
                          ? expirationTime.Value.DateTime.Subtract(DateTime.Now)
                          : (TimeSpan?)null;

            var isSet = _database.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
    }
}
