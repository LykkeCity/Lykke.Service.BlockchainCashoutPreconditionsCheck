using Lykke.Service.BlockchainWallets.Client;
using Lykke.Service.BlockchainWallets.Contract.Models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Common;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class AddressExtensionService
    {
        private readonly TimeSpan _cacheTime;
        private readonly IBlockchainWalletsClient _blockchainWalletsClient;
        private readonly IDistributedCache _distributedCache;

        public AddressExtensionService(
            IBlockchainWalletsClient blockchainWalletsClient,
            IDistributedCache distributedCache,
            TimeSpan cacheTime)
        {
            _cacheTime = cacheTime;
            _blockchainWalletsClient = blockchainWalletsClient;
            _distributedCache = distributedCache;
        }

        public async Task<AddressExtensionConstantsResponse> TryGetAddressExtensionConstantsAsync(string blockchainType)
        {
            if (string.IsNullOrEmpty(blockchainType))
            {
                throw new ArgumentException("Should not be null or empty.", nameof(blockchainType));
            }

            var cachedConstants = await _distributedCache.TryGetFromCache(
                $"AddressExtensionConstants-{blockchainType}",
                async () => await _blockchainWalletsClient.TryGetAddressExtensionConstantsAsync(blockchainType),
                _cacheTime
            );

            // For the case when BIL service is unavailable, and we've got null as the value after the above operation,
            // we need to remove it back. Otherwise, at the next call to TryGetFromCache(...) we will obtain null even
            // if the BIL service is available now, unless the cache timeout expires.
            if (cachedConstants == null)
                await _distributedCache.RemoveAsync($"AddressExtensionConstants-{blockchainType}");

            return cachedConstants;
        }
    }

    public static class CacheExt
    {
        public static async Task<T> TryGetFromCache<T>(this IDistributedCache cache, string key,
            Func<Task<T>> getRecordFunc, TimeSpan expiration)
        {
            var (isCached, record) = await TryGetRecordFromCache<T>(cache, key);

            if (!isCached)
            {
                record = await getRecordFunc();
                await TryUpdateRecordInCache(cache, key, record, expiration);
            }

            return record;
        }

        private static async Task<(bool, T)> TryGetRecordFromCache<T>(IDistributedCache cache, string key)
        {
            var value = await cache.GetStringAsync(key);

            if (value != null)
            {
                return (true, value.DeserializeJson<T>());
            }

            return (false, default(T));
        }

        private static async Task TryUpdateRecordInCache<T>(IDistributedCache cache, string key, T record, TimeSpan? expiration)
        {
            await cache.SetStringAsync(key, record.ToJson(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration });
        }
    }
}
