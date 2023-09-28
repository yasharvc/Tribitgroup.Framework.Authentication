namespace Tribitgroup.Framewok.Shared.Interfaces
{
    public interface ICache<TKey, TValue>
    {
        TValue Get(TKey key);
        Task AddOrUpdate(TKey key, TValue value, TimeSpan? expiration = null);
        Task Remove(TKey key);
        Task<bool> ContainsKey(TKey key);
        Task Clear();
    }
}