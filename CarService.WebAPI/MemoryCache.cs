
using CarService.WebAPI.Data;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace CarService.WebAPI
{
    public interface ImCache
    {
        void CreateCache(List<Car> model, string key);
        void RemoveFromCacheById(int id, string key);
        List<Car> GetCacheByKey(string key);
        void RemoveCacheByKey(string key);
    }
    public class MemoryCache : ImCache
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void CreateCache(List<Car> model, string key)
        {
            var getCache = GetCacheByKey(key);
            if (getCache is null)
                _memoryCache.Set(key, model);
        }
        public List<Car> GetCacheByKey(string key)
        {
            var data = _memoryCache.Get(key);
            return (List<Car>)data;
        }
        public void RemoveFromCacheById(int id, string key)
        {
            var cache = ((List<Car>)_memoryCache.Get(key)).Where(x => x.Id != id).ToList();
            _memoryCache.Set(key, cache);
        }
        public void RemoveCacheByKey(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
