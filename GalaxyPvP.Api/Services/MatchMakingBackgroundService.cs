using Azure.Core;
using GalaxyPvP.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace GalaxyPvP.Api.Services
{
    public class MatchMakingBackgroundService : BackgroundService
    {
        private readonly ILogger<MatchMakingBackgroundService> _logger;
        private readonly IMemoryCache _memoryCache;

        public MatchMakingBackgroundService(ILogger<MatchMakingBackgroundService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Dictionary<string, MatchMakingTicket> PlayerPools = GetCachedDictionary();

                if (PlayerPools == null)
                {
                    PlayerPools = new Dictionary<string, MatchMakingTicket>();
                    SaveDictionaryToCache(PlayerPools);
                }

                _logger.LogInformation($"Counter: {PlayerPools.Count}");

                //if (PlayerPools.ContainsKey(request.PlayerId))
                //{
                //    if (PlayerPools.Count >= 6)
                //    {
                //        _logger.LogInformation($"Counter: {PlayerPools.Count}");
                //    }
                //    else if (PlayerPools.Count >= 4 && request.WaitTime >= 30)
                //    {
                //        _logger.LogInformation($"Counter: {PlayerPools.Count}");
                //    }

                //    _logger.LogInformation($"Counter: {PlayerPools.Count}");
                //}
                //else
                //{
                //    _logger.LogInformation($"Counter: {PlayerPools.Count}");
                //}

                // Adjust the delay to run every 5 seconds
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        Dictionary<string, MatchMakingTicket> GetCachedDictionary()
        {
            // Try to get the dictionary from the cache
            if (_memoryCache.TryGetValue("PlayerPools", out Dictionary<string, MatchMakingTicket> cachedData))
            {
                return cachedData;
            }

            // If not in the cache, return null or fetch it from the data source
            return null;
        }

        void SaveDictionaryToCache(Dictionary<string, MatchMakingTicket> data)
        {
            // Set the dictionary in the cache with a specific key and expiration time
            _memoryCache.Set("PlayerPools", data);
        }
    }
}
