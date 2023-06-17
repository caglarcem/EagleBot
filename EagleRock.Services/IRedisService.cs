namespace EagleRock.Services
{
    public interface IRedisService
    {
        Task<string> GetValue(string key);

        Task SetValue(string key, string value);

        Task<IEnumerable<string>> GetValuesByKeyPattern(string pattern);

        Task<IEnumerable<string>> GetValuesByKeyMinimumValue(string pattern, char keyDelimeter, short partNo, double minValue);
    }
}
