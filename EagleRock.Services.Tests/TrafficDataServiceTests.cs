using EagleRock.Services.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace EagleRock.Services.Tests
{
    public class TrafficDataServiceTests
    {
        private readonly Mock<IRedisService> _mockRedisService = new Mock<IRedisService>();

        [Fact]
        public async Task ShouldGetAllTrafficData()
        {
            // Given Redis cache with traffic data values
            var redisTrafficData = new List<string>
            {
                "{\"EagleBotId\":\"1001\",\"RoadName\":\"Henley\",\"TrafficDirection\":2,\"VehiclesPerMinute\":110,\"AverageVehicleSpeed\":50.0,\"BotLocation\":{\"Latitude\":0.32423,\"Longitude\":0.63463},\"Timestamp\":\"2023-06-17T20:03:43.5603515+10:00\"}",
                "{\"EagleBotId\":\"65644\",\"RoadName\":\"Zillmere\",\"TrafficDirection\":2,\"VehiclesPerMinute\":54,\"AverageVehicleSpeed\":56.0,\"BotLocation\":null,\"Timestamp\":\"2023-06-16T18:55:34.2595559+10:00\"}",
                "{\"EagleBotId\":\"667\",\"RoadName\":\"Calis\",\"TrafficDirection\":3,\"VehiclesPerMinute\":144,\"AverageVehicleSpeed\":56.88,\"BotLocation\":{\"Latitude\":38.8951,\"Longitude\":-77.0364},\"Timestamp\":\"2023-06-16T22:10:05.9661852+10:00\"}"
            };

            _mockRedisService.Setup(x => x.GetValuesByKeyPattern("EagleBot_*")).ReturnsAsync(redisTrafficData);

            var trafficDataService = GetTrafficDataService(_mockRedisService);

            // When all traffic data is requested
            var result = await trafficDataService.GetAllTrafficData();

            // Then all 3 traffic data is transformed as TrafficDataModel
            Assert.Equal(3, result.Count());

            var eagleBot1 = result.FirstOrDefault(x => x.EagleBotId == "1001");
            Assert.NotNull(eagleBot1);
            Assert.Equal("Henley", eagleBot1.RoadName);
            Assert.Equal((Direction)2, eagleBot1.TrafficDirection);
            Assert.Equal(110, eagleBot1.VehiclesPerMinute);
            Assert.Equal(50.0, eagleBot1.AverageVehicleSpeed);

            var eagleBot2 = result.FirstOrDefault(x => x.EagleBotId == "65644");
            Assert.NotNull(eagleBot2);
            Assert.Equal("Zillmere", eagleBot2.RoadName);
            Assert.Equal((Direction)2, eagleBot2.TrafficDirection);
            Assert.Equal(54, eagleBot2.VehiclesPerMinute);
            Assert.Equal(56.0, eagleBot2.AverageVehicleSpeed);

            var eagleBot3 = result.FirstOrDefault(x => x.EagleBotId == "667");
            Assert.NotNull(eagleBot3);
            Assert.Equal("Calis", eagleBot3.RoadName);
            Assert.Equal((Direction)3, eagleBot3.TrafficDirection);
            Assert.Equal(144, eagleBot3.VehiclesPerMinute);
            Assert.Equal(56.88, eagleBot3.AverageVehicleSpeed);
        }

        [Fact]
        public async Task ShouldGetLatestTrafficDataOfActiveBots()
        {
            // Given Redis cache with traffic data values
            var redisTrafficData = new List<string>
            {
                "{\"EagleBotId\":\"1001\",\"RoadName\":\"Henley\",\"TrafficDirection\":2,\"VehiclesPerMinute\":110,\"AverageVehicleSpeed\":50.0,\"BotLocation\":{\"Latitude\":0.32423,\"Longitude\":0.63463},\"Timestamp\":\"2023-06-17T20:03:43.5603515+10:00\"}",
                "{\"EagleBotId\":\"1001\",\"RoadName\":\"Cesme\",\"TrafficDirection\":1,\"VehiclesPerMinute\":120,\"AverageVehicleSpeed\":64.0,\"BotLocation\":{\"Latitude\":0.32423,\"Longitude\":0.63463},\"Timestamp\":\"2023-06-17T22:03:43.5603515+10:00\"}",
                "{\"EagleBotId\":\"65644\",\"RoadName\":\"Zillmere\",\"TrafficDirection\":2,\"VehiclesPerMinute\":54,\"AverageVehicleSpeed\":56.0,\"BotLocation\":null,\"Timestamp\":\"2023-06-16T18:55:34.2595559+10:00\"}",
                "{\"EagleBotId\":\"667\",\"RoadName\":\"Calis\",\"TrafficDirection\":3,\"VehiclesPerMinute\":144,\"AverageVehicleSpeed\":56.88,\"BotLocation\":{\"Latitude\":38.8951,\"Longitude\":-77.0364},\"Timestamp\":\"2023-06-16T22:10:05.9661852+10:00\"}"
            };

            _mockRedisService.Setup(x => x.GetValuesByKeyMinimumValue("EagleBot_*", '_', 3, It.IsAny<double>())).ReturnsAsync(redisTrafficData);

            var trafficDataService = GetTrafficDataService(_mockRedisService);

            // When latest traffic data is requested
            var result = await trafficDataService.GetLatestTrafficDataOfActiveBots();

            // Then latest data of each eagle bot is returned
            Assert.Equal(3, result.Count());

            var eagleBot1Data = result.FirstOrDefault(x => x.EagleBotId == "1001");
            Assert.NotNull(eagleBot1Data);
            Assert.Equal("Cesme", eagleBot1Data.RoadName);
            Assert.Equal((Direction)1, eagleBot1Data.TrafficDirection);
            Assert.Equal(120, eagleBot1Data.VehiclesPerMinute);
            Assert.Equal(64.0, eagleBot1Data.AverageVehicleSpeed);

            var eagleBot2Data = result.FirstOrDefault(x => x.EagleBotId == "65644");
            Assert.NotNull(eagleBot2Data);
            Assert.Equal("Zillmere", eagleBot2Data.RoadName);
            Assert.Equal((Direction)2, eagleBot2Data.TrafficDirection);
            Assert.Equal(54, eagleBot2Data.VehiclesPerMinute);
            Assert.Equal(56.0, eagleBot2Data.AverageVehicleSpeed);

            var eagleBot3Data = result.FirstOrDefault(x => x.EagleBotId == "667");
            Assert.NotNull(eagleBot3Data);
            Assert.Equal("Calis", eagleBot3Data.RoadName);
            Assert.Equal((Direction)3, eagleBot3Data.TrafficDirection);
            Assert.Equal(144, eagleBot3Data.VehiclesPerMinute);
            Assert.Equal(56.88, eagleBot3Data.AverageVehicleSpeed);
        }

        [Fact]
        public async Task ShouldGetLatestTrafficDataByEagleBot()
        {
            // Given Redis cache with traffic data values
            var redisTrafficData = new List<string>
            {
                "{\"EagleBotId\":\"65644\",\"RoadName\":\"Zillmere\",\"TrafficDirection\":1,\"VehiclesPerMinute\":77,\"AverageVehicleSpeed\":60.0,\"BotLocation\":null,\"Timestamp\":\"2023-06-16T19:32:34.2595559+10:00\"}",
                "{\"EagleBotId\":\"65644\",\"RoadName\":\"Battersby\",\"TrafficDirection\":3,\"VehiclesPerMinute\":67,\"AverageVehicleSpeed\":65.0,\"BotLocation\":null,\"Timestamp\":\"2023-06-16T18:55:34.2595559+10:00\"}",
                "{\"EagleBotId\":\"65644\",\"RoadName\":\"Handford\",\"TrafficDirection\":2,\"VehiclesPerMinute\":54,\"AverageVehicleSpeed\":78.0,\"BotLocation\":null,\"Timestamp\":\"2023-06-16T21:18:34.2595559+10:00\"}"
            };

            _mockRedisService.Setup(x => x.GetValuesByKeyPattern("EagleBot_65644_*")).ReturnsAsync(redisTrafficData);

            var trafficDataService = GetTrafficDataService(_mockRedisService);

            // When latest traffic data is requested
            var trafficData = await trafficDataService.GetLatestTrafficDataByEagleBot("65644");

            // Then latest data of the eagle bot is returned based on timestamp
            Assert.NotNull(trafficData);
            Assert.Equal("Handford", trafficData.RoadName);
            Assert.Equal((Direction)2, trafficData.TrafficDirection);
            Assert.Equal(54, trafficData.VehiclesPerMinute);
            Assert.Equal(78.0, trafficData.AverageVehicleSpeed);
        }

        [Fact]
        public async Task ShouldSaveTrafficData()
        {
            // Given new traffic data
            var trafficData = new TrafficDataModel
            {
                EagleBotId = "34255",
                RoadName = "Sandgate",
                AverageVehicleSpeed = 74.7,
                TrafficDirection = Direction.WEST,
                VehiclesPerMinute = 89,
                Timestamp = DateTime.UtcNow,
            };

            var trafficDataService = GetTrafficDataService(_mockRedisService);

            // When the traffic data is posted
            await trafficDataService.SaveTrafficData(trafficData);

            // Then the new traffic data is saved
            _mockRedisService.Verify(
                d => d.SetValue(It.Is<string>(x => x.StartsWith("EagleBot_34255_"))
                , It.Is<string>(x => x.StartsWith("{\"EagleBotId\":\"34255\",\"RoadName\":\"Sandgate\",\"TrafficDirection\":3,\"VehiclesPerMinute\":89,\"AverageVehicleSpeed\":74.7")))
                , Times.Once); // Verify that save method on Redis service was called
        }

        private ITrafficDataService GetTrafficDataService(Mock<IRedisService> redisService)
        {
            return new TrafficDataService(redisService.Object, new Mock<ILogger<TrafficDataService>>().Object);
        }
    }
}