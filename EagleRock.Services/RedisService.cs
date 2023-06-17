using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisService(IConfiguration configuration)
        {
            var connectionString = configuration["ServiceUrls:RedisService"];
            _redisConnection = ConnectionMultiplexer.Connect(connectionString);
        }

        public async Task SetValue(string key, string value)
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value);
        }

        public async Task<IEnumerable<string>> GetValuesByKeyPattern(string pattern)
        {
            var db = _redisConnection.GetDatabase();

            var keys = new List<RedisKey>();

            var scanKeys = GetScanKeys(pattern);

            keys.AddRange(Array.ConvertAll(scanKeys, key => (RedisKey)key));

            var values = await db.StringGetAsync(keys.ToArray());

            return values.Where(x => x != RedisValue.Null).ToArray().ToStringArray();
        }

        public async Task<IEnumerable<string>> GetValuesByKeyMinimumValue(string pattern, char keyDelimeter, short partNo, double minValue)
        {
            var db = _redisConnection.GetDatabase();

            var keys = new List<RedisKey>();

            var scanKeys = GetScanKeys(pattern);

            foreach (var scanKey in scanKeys)
            {
                var key = (string)scanKey;

                var keyParts = key.Split(keyDelimeter);

                if (keyParts.Length >= partNo && double.TryParse(keyParts[partNo - 1], out double numericValue))
                {
                    if (numericValue > minValue)
                    {
                        keys.Add(key);
                    }
                }
            }

            var values = await db.StringGetAsync(keys.ToArray());

            return values.Where(x => x != RedisValue.Null).ToArray().ToStringArray();

        }

        private RedisResult[] GetScanKeys(string pattern)
        {
            var db = _redisConnection.GetDatabase();
            var cursor = 0L;
            var batchSize = 1000;

            List<RedisResult> scanKeysList = new();

            do
            {
                // Improve performance by directly executing command rather than fetching and iterating all the keys
                var batch = db.CreateBatch();
                var scanResult = (RedisResult[])db.Execute("SCAN", cursor, "MATCH", pattern, "COUNT", batchSize);
                batch.Execute();

                cursor = (long)scanResult[0];

                var scanKeys = (RedisResult[])scanResult[1];

                foreach(var scanKey in scanKeys)
                {
                    scanKeysList.Add(scanKey);
                }
            }
            while (cursor != 0);

            return scanKeysList.Where(x => x != null).ToArray();
        }

    }
}
