
using System.Collections.Generic;

namespace LRUCache.Cache
{
    public interface ILRUCache<K, V>
    {
        int Capacity { get; }
        int Count { get; }
        void Add(K key, V value);
        bool ContainsKey(K key);
        bool TryGetValue(K key, out V value);
        void Clear();
        List<V> GetAllValues();
        K GetKeyByValue(V value);
    }
}
