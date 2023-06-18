using EagleRock.Services.Interfaces;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly IRedisCommand _redisCommand;

        public RedisService(IDatabase database, IRedisCommand redisCommand)
        {
            _database = database;
            _redisCommand = redisCommand;
        }

        public async Task SetValue(string key, string value)
        {
            await _database.StringSetAsync(key, value);

            string channelName = "event_channel";

            // Publish to Redis subscriber to send to RabbitMQ
            _database.Publish(channelName, value);
        }

        public async Task<IEnumerable<string>> GetValuesByKeyPattern(string pattern)
        {
            var keys = new List<RedisKey>();

            var scanKeys = GetScanKeys(pattern);

            keys.AddRange(Array.ConvertAll(scanKeys, key => (RedisKey)key));

            var values = await _database.StringGetAsync(keys.ToArray());

            return values.Where(x => x != RedisValue.Null).ToArray().ToStringArray();
        }

        public async Task<IEnumerable<string>> GetValuesByKeyMinimumValue(string pattern, char keyDelimeter, short partNo, double minValue)
        {
            var keys = new List<RedisKey>();

            var scanKeys = GetScanKeys(pattern);

            foreach (var scanKey in scanKeys)
            {
                var keyParts = scanKey.Split(keyDelimeter);

                if (keyParts.Length >= partNo && double.TryParse(keyParts[partNo - 1], out double numericValue))
                {
                    if (numericValue > minValue)
                    {
                        keys.Add(scanKey);
                    }
                }
            }

            var values = await _database.StringGetAsync(keys.ToArray());

            return values.Where(x => x != RedisValue.Null).ToArray().ToStringArray();

        }

        private string[] GetScanKeys(string pattern)
        {
            var cursor = 0L;
            var batchSize = 1000;

            List<string> scanKeysList = new();

            do
            {
                var scanKeys = Array.Empty<string>();

                // Improved performance by executing command rather than GetServer().Keys();
                (cursor, scanKeys) = _redisCommand.Execute(cursor, pattern, batchSize);

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
