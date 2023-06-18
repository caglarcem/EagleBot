namespace EagleRock.Services.Interfaces
{
    public interface IRedisCommand
    {
        (long Cursor, string[] ScanKeys) Execute(long cursor, string pattern, int batchSize);
    }
}
