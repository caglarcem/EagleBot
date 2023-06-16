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

        public async Task<IEnumerable<string>> GetValuesByKeyPattern(string pattern)
        {
            var keys = new List<RedisKey>();
            var db = _redisConnection.GetDatabase();
            var cursor = 0L;

            do
            {
                // Better performance by directly executing command
                var scanResult = (RedisResult[])db.Execute("SCAN", cursor, "MATCH", pattern);

                cursor = (long)scanResult[0];

                var scanKeys = (RedisResult[])scanResult[1];

                keys.AddRange(Array.ConvertAll(scanKeys, key => (RedisKey)key));
            }
            while (cursor != 0);

            var values = await db.StringGetAsync(keys.ToArray());
            return values.Where(x => x != RedisValue.Null).ToArray().ToStringArray();
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
