using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LRUCache.Models;
using LRUCache.Cache;
using Microsoft.Extensions.Configuration;

namespace LRUCache.Services
{
    public class CacheService:ICacheService
    {
        private readonly IConfiguration configuration;
        private readonly int capacity;
        public CacheService(IConfiguration con)
        {
            configuration = con;
            capacity = Convert.ToInt32(configuration["CacheCapacity"]);
            cache = new LRUCache<int, Student>(capacity);
        }
        private readonly LRUCache<int, Student> cache;
        public LRUCache<int,Student> GetCache()
        {
            return cache;
        }
    }
}
