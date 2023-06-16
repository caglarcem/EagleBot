namespace EagleRock.Services
{
    public interface IRedisService
    {
        IEnumerable<string> GetAllKeys();

        Task<string> GetValue(string key);

        Task SetValue(string key, string value);
    }
}
