using System;
using LRUCache.Models;
using LRUCache.Cache;

namespace LRUCache.Services
{
    public interface ICacheService
    {
        LRUCache<int, Student> GetCache();
       
    }
}
