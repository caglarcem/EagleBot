using EagleRock.Services.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class TrafficDataService : ITrafficDataService
    {
        private readonly IRedisService _redisService;
        private readonly ILogger<TrafficDataService> _logger;

        private const string EAGLEBOT_REDIS_KEY_PREFIX = "EagleBot";

        public TrafficDataService(IRedisService redisService, ILogger<TrafficDataService> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<IEnumerable<TrafficDataModel>> GetAllTrafficData()
        {
            try
            {
                // Add wildcard to get EagleBot related data only in case there are other entries in cache.
                var values = await _redisService.GetValuesByKeyPattern($"{EAGLEBOT_REDIS_KEY_PREFIX}_*");

                if(!(values?.Any() == true))
                {
                    return Enumerable.Empty<TrafficDataModel>();
                }

                var trafficDataModels = GetTrafficDataModelsFromRedisValues(values);

                return trafficDataModels;
            }
            catch (Exception ex) when (ex is RedisConnectionException || ex is RedisTimeoutException)
            {
                _logger.LogError(ex, $"Cannot connect to Redis database.");
                throw;
            }
            catch (JsonSerializationException jex)
            {
                _logger.LogError(jex, $" Redis value cannot be deserialized to {nameof(TrafficDataModel)}.");
                throw;
            }
        }

        public async Task<IEnumerable<TrafficDataModel>> GetLatestTrafficDataOfActiveBots()
        {
            try
            {
                // Compare total microseconds to find out minimum an hour ago
                var totalMicroseconds = (DateTime.Now.AddHours(-1) - DateTime.MinValue).TotalMilliseconds * 1000;

                // Key format: EagleBot_1234_{totalMicroseconds}". 3rd item is the milliseconds to be compared
                var values = await _redisService.GetValuesByKeyMinimumValue($"{EAGLEBOT_REDIS_KEY_PREFIX}_*", '_', 3, totalMicroseconds);

                var trafficDataModels = GetTrafficDataModelsFromRedisValues(values);

                var latestTrafficData = trafficDataModels.GroupBy(item => item.EagleBotId)
                                 .Select(group => group.OrderByDescending(item => item.Timestamp).First());

                return latestTrafficData;

            }
            catch (Exception ex) when(ex is RedisConnectionException || ex is RedisTimeoutException)
            {
                _logger.LogError(ex, $"Cannot connect to Redis database.");
                throw;
            }
            catch (JsonSerializationException jex)
            {
                _logger.LogError(jex, $" Redis value cannot be deserialized to {nameof(TrafficDataModel)}.");
                throw;
            }
        }

        public async Task<TrafficDataModel> GetLatestTrafficDataByEagleBot(string eagleBotId)
        {
            try
            {
                var values = await _redisService.GetValuesByKeyPattern($"{EAGLEBOT_REDIS_KEY_PREFIX}_{eagleBotId}_*");

                if (!(values?.Any() == true))
                {
                    return null;
                }

                var trafficDataModels = GetTrafficDataModelsFromRedisValues(values);

                return trafficDataModels.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            }
            catch (Exception ex) when (ex is RedisConnectionException || ex is RedisTimeoutException)
            {
                _logger.LogError(ex, $"Cannot connect to Redis database.");
                throw;
            }
            catch (JsonSerializationException jex)
            {
                _logger.LogError(jex, $" Redis value cannot be deserialized to {nameof(TrafficDataModel)}.");
                throw;
            }
        }

        public async Task SaveTrafficData(TrafficDataModel trafficData)
        {
            try
            {
                if (trafficData == null) return;

                trafficData.Timestamp = DateTime.Now;

                var rawTrafficData = JsonConvert.SerializeObject(trafficData);

                // Make the cache key unique, by using the total microseconds elapsed since min datetime value
                var totalMicroseconds = (trafficData.Timestamp - DateTime.MinValue).TotalMilliseconds * 1000;
                var key = $"{EAGLEBOT_REDIS_KEY_PREFIX}_{trafficData.EagleBotId}_{totalMicroseconds}";

                await _redisService.SetValue(key, rawTrafficData);
            }
            catch (Exception ex) when (ex is RedisConnectionException || ex is RedisTimeoutException)
            {
                _logger.LogError(ex, $"Cannot connect to Redis database.");
                throw;
            }
            catch (JsonSerializationException jex)
            {
                _logger.LogError(jex, $" Redis value cannot be serialized.");
                throw;
            }
        }

        private IEnumerable<TrafficDataModel> GetTrafficDataModelsFromRedisValues(IEnumerable<string> values)
        {
            var trafficDataModels = new List<TrafficDataModel>();

            foreach (var rawTrafficData in values)
            {
                var trafficData = JsonConvert.DeserializeObject<TrafficDataModel>(rawTrafficData);

                if (trafficData != null)
                {
                    trafficDataModels.Add(trafficData);
                }
            }

            return trafficDataModels;
        }
    }
}