using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly ConfigurationOptions _configurationOptions;

        public RedisService(IConfiguration configuration)
        {
            var connectionString = configuration["ServiceUrls:RedisService"];
            _configurationOptions = ConfigurationOptions.Parse(connectionString);
            _redisConnection = ConnectionMultiplexer.Connect(connectionString);
        }

        public IEnumerable<string> GetAllKeys()
        {
            // TODO improve performance by executing SCAN command directly and batching

            var db = _redisConnection.GetDatabase();

            var keys = _redisConnection.GetServer(_configurationOptions.EndPoints.First()).Keys(database: db.Database);

            return keys.Select(x => x.ToString());
        }

        public async Task<string> GetValue(string key)
        {
            var db = _redisConnection.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetValue(string key, string value)
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value);
        }
    }
}
