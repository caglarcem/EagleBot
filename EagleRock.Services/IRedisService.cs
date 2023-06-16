namespace EagleRock.Services
{
    public interface IRedisService
    {
        Task<IEnumerable<string>> GetValuesByKeyPattern(string pattern);

        Task<string> GetValue(string key);

        Task SetValue(string key, string value);
    }
}
