namespace LoginPageAPI.Services
{
    /// <summary>
    /// Caching operations abstraction.
    /// </summary>
    public interface ICachingService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
    }
}
