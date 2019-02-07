using System;
using Microsoft.Extensions.Caching.Memory;

namespace Askmethat.Aspnet.JsonLocalizer.MemoryManagement
{
    public class CustomMemoryManager : IMemoryCache
    {
        
        public ICacheEntry CreateEntry(object key)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(object key, out object value)
        {
            throw new System.NotImplementedException();
        }
    }
}