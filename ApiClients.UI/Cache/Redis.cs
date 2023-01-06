using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;

namespace Api.Cache;

public class Redis
{
    private readonly IDistributedCache _distributedCache;
    private readonly DistributedCacheEntryOptions opts;

    public Redis(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
        opts = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
                    //.SetSlidingExpiration(TimeSpan.FromMinutes(2));
    }

    public string Get(string key) 
    {
        try
        {
            byte[] value = _distributedCache.Get(key);

            if (value != null)
            {
                return Encoding.UTF8.GetString(value);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return string.Empty;
    }

    public void Set(string key, string value)
    {
        try
        {
            _distributedCache.Set(key, Encoding.UTF8.GetBytes(value), opts);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
