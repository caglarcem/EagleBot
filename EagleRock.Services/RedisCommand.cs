using EagleRock.Services.Interfaces;
using StackExchange.Redis;

namespace EagleRock.Services
{
    public class RedisCommand : IRedisCommand
    {
        private IDatabase _database;

        public RedisCommand(IDatabase database)
        {
            _database = database;
        }

        // We need this service to test database execute. Otherwise this cannot be stubbed (primitive types cannot be cast to RedisResult)
        public (long Cursor, string[] ScanKeys) Execute(long cursor, string pattern, int batchSize)
        {
            var batch = _database.CreateBatch();
            var scanResult = (RedisResult[])_database.Execute("SCAN", cursor, "MATCH", pattern, "COUNT", batchSize);
            batch.Execute();

            return (Cursor: (long)scanResult[0], (string[])scanResult[1]);
        }
    }
}
