using EagleRock.Services.Interfaces;
using Moq;
using StackExchange.Redis;

namespace EagleRock.Services.Tests
{
    public class RedisServiceTests
    {
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<IRedisCommand> _mockRedisCommand;

        public RedisServiceTests()
        {
            _mockDatabase = new Mock<IDatabase>();
            _mockRedisCommand = new Mock<IRedisCommand>();
        }

        [Fact]
        public async Task ShouldSetValue()
        {
            // Given a key and value
            var key = "testKey";
            var value = "testValue";

            _mockDatabase.Setup(db => db.StringSetAsync(key, value, null, When.Always, CommandFlags.None))
                             .Returns(Task.FromResult(true));

            var scanKeys = new string[] { "testOther", "testMyKey", "testAnyKey" };

            _mockRedisCommand.Setup(x => x.Execute(0, "test*", 100)).Returns((0, scanKeys));

            var redisService = new RedisService(_mockDatabase.Object, _mockRedisCommand.Object);

            // When values are set in redis
            await redisService.SetValue(key, value);

            // Then the SetString method is called
            _mockDatabase.Verify(db => db.StringSetAsync(key, value, null, When.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task ShouldGetValuesByKeyPattern()
        {
            // Given a key pattern
            var pattern = "key*";
            var cursor = 0L;

            //  and keys and values in Redis
            var scanKeys = new string[] { "key1", "key2", "tea"};

            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey[]>(), CommandFlags.None))
                             .ReturnsAsync(new RedisValue[]
                             {
                                "value1",
                                "value2"
                             });

            _mockRedisCommand.Setup(x => x.Execute(cursor, "key*", 100)).Returns((0, new string[] { "key1", "key2" }));

            var redisService = new RedisService(_mockDatabase.Object, _mockRedisCommand.Object);

            // When values by pattern are requested
            var result = await redisService.GetValuesByKeyPattern(pattern);

            // Then the values are returned
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains("value1", result);
            Assert.Contains("value2", result);
        }

        [Fact]
        public async Task ShouldGetValuesByKeyMinimumValue()
        {
            // Given a key pattern
            var pattern = "key*";
            var cursor = 0L;

            //  and keys and values in Redis
            var scanKeys = new string[] { "key_1000", "key_1011", "key_1022", "tea_1022" };

            _mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey[]>(), CommandFlags.None))
                             .ReturnsAsync(new RedisValue[]
                             {
                                "value_1011",
                                "value_1022"
                             });

            _mockRedisCommand.Setup(x => x.Execute(cursor, "key*", 1000)).Returns((0, new string[] { "key_1011", "key_1022" }));

            var redisService = new RedisService(_mockDatabase.Object, _mockRedisCommand.Object);

            // When values by minimum number are requested
            var result = await redisService.GetValuesByKeyMinimumValue(pattern, '_', 2, 1001);

            // Then the values with greater number are returned
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains("value_1011", result);
            Assert.Contains("value_1022", result);
        }
    }
}