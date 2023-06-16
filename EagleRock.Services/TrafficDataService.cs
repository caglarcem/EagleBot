using EagleRock.Services.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class TrafficDataService : ITrafficDataService
    {
        private readonly IRedisService _redisService;

        private const string EAGLEBOT_REDIS_KEY_PREFIX = "EagleBot_";

        public TrafficDataService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<IEnumerable<TrafficDataModel>> GetAllTrafficData()
        {
            var keys = _redisService.GetAllKeys().Where(x => x.StartsWith(EAGLEBOT_REDIS_KEY_PREFIX));

            var trafficDataModels = new List<TrafficDataModel>();

            // Key here is EagleBot_{EagleBotId}_{Timestamp}
            foreach(var key in keys)
            {
                var rawTrafficData = await _redisService.GetValue(key);

                var trafficData = JsonConvert.DeserializeObject<TrafficDataModel>(rawTrafficData);

                if (trafficData != null)
                {
                    trafficDataModels.Add(trafficData);
                }
            }

            return new List<TrafficDataModel>();
        }

        public async Task<string> SaveTrafficData(TrafficDataModel trafficData)
        {
            trafficData.Timestamp = DateTime.Now;

            var rawTrafficData = JsonConvert.SerializeObject(trafficData);

            if (rawTrafficData != null)
            {
                // Keep the cache key unique, by large number represented in miliseconds interval
                var totalMilliseconds = (trafficData.Timestamp - DateTime.MinValue).TotalMilliseconds;
                var key = $"{EAGLEBOT_REDIS_KEY_PREFIX}{trafficData.EagleBotId}_{totalMilliseconds}";

                await _redisService.SetValue(key, rawTrafficData);

                return key;
            }

            return string.Empty;
        }
    }
}