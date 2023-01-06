using Api.Cache;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly string cacheName = "Notification";
        private readonly Redis _cache;
        
        public NotificationController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _cache = new Redis(_distributedCache);
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult Receive([FromBody] List<Notification> notifications)
        {
            string cacheValue = _cache.Get(cacheName);

            if (!string.IsNullOrEmpty(cacheValue))
            {    
                List<Notification> notifs = JsonConvert.DeserializeObject<List<Notification>>(cacheValue);
                notifications.AddRange(notifs);
            }

            _cache.Set(cacheName, JsonConvert.SerializeObject(notifications));

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Read()
        {
            List<Notification> notifications = new List<Notification>();

            string cacheValue = _cache.Get(cacheName);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                notifications.AddRange(JsonConvert.DeserializeObject<List<Notification>>(cacheValue));
            }

            return Ok(notifications);
        }
    }
}
