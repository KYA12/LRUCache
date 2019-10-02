using System;
using System.Collections.Generic;
using System.Linq;

namespace LRUCache.Cache
{
    public class LRUCache<K, V> : ILRUCache<K, V>
    {
        private object lockObj = new object();
        private int capacity;
        private Dictionary<K, Entry> cacheMap;
        private LruNode oldestNode;
        private LRUCache() { }
        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            cacheMap = new Dictionary<K, Entry>(capacity);
        }
        public int Capacity { get { return capacity; } }
        public int Count { get { return cacheMap.Count; } }
        public void Clear()
        {
            lock (lockObj)
            {
                this.cacheMap.Clear();
                var node = this.oldestNode;
                this.oldestNode = null;
                while (node != null)
                {
                    var temp = node;
                    node = node.Prev;
                    temp.Clear();
                }
            }
        }
        private Entry GetObject(K key)
        {
            if (cacheMap.ContainsKey(key))
            return cacheMap[key];
            return default(Entry);
        }
        public List<V> GetAllValues()
        {
            lock (lockObj)
            {
                List<V> values = new List<V>();
                K[] array = new K[cacheMap.Keys.Count];
                this.cacheMap.Keys.CopyTo(array, 0);
                for (int i = 0; i < cacheMap.Keys.Count; i++)
                {
                    Entry entry = GetObject(array[i]);
                    values.Add(entry.value);
                }

                return values;
            }
        }
        public bool ContainsKey(K key)
        {
            lock (lockObj)
            {
                return this.cacheMap.ContainsKey(key);
            }
        }
        public bool TryGetValue(K key, out V value)
        {
            lock (lockObj)
            {
                Entry entry;
                if (this.cacheMap.TryGetValue(key, out entry))
                {
                    Touch(entry.node);
                    value = entry.value;
                    return true;
                }
            }
            value = default(V);
            return false;
        }
        public void Add(K key, V value)
        { 
            lock (lockObj)
            {
                Entry entry;
                if (!this.cacheMap.TryGetValue(key, out entry))
                {
                    LruNode node;
                    if (this.cacheMap.Count >= capacity)
                    {
                        node = this.oldestNode;
                        this.cacheMap.Remove(node.Key);
                        node.Key = key;
                        this.oldestNode = node.Prev;
                    }
                    else
                    {
                        node = new LruNode(key);
                        if (this.oldestNode != null)
                        {
                            node.InsertAfter(this.oldestNode);
                        }
                        else
                        {
                            this.oldestNode = node;
                        }
                    }
                    entry.node = node;
                    entry.value = value;
                    this.cacheMap.Add(key, entry);
                }
                else
                {
                    entry.value = value;
                    this.cacheMap[key] = entry;
                    Touch(entry.node);
                }
            }
        }
        private void Touch(LruNode node)
        {
            if (node != this.oldestNode)
            {
                node.MoveAfter(this.oldestNode);
            }
            else
            {
                this.oldestNode = node.Prev;
            }
        }
        private struct Entry
        {
            public LruNode node;
            public V value;
            public Entry(LruNode node, V value)
            {
                this.node = node;
                this.value = value;
            }
        }
        public K GetKeyByValue(V value)
        {
            K key = (from p in cacheMap
                        where p.Value.value.Equals(value)
                        select p.Key).FirstOrDefault();
            return key;
        }
        private class LruNode
        {
            public K Key { get; set; }
            public LruNode Prev { get; private set; }
            public LruNode Next { get; private set; }
            public LruNode(K key)
            {
                Key = key;
                Prev = Next = this;
            }
            public void MoveAfter(LruNode node)
            {
                if (node.Next != this)
                {
                    this.Next.Prev = this.Prev;
                    this.Prev.Next = this.Next;
                    InsertAfter(node);
                }
            }
            public void InsertAfter(LruNode node)
            {
                this.Prev = node;
                this.Next = node.Next;
                node.Next = this.Next.Prev = this;
            }
            public void Clear()
            {
                Key = default(K);
                Prev = Next = null;
            }
            public override string ToString()
            {
                return "LruNode<" + Key.ToString() + ">";
            }
        }
    }
}
