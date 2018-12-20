using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainWallets.Contract;
using MoreLinq;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class BlockchainWalletsCacheService : IBlockchainWalletsCacheService, IInitBlockchainWalletsCacheService
    {
        private bool _canBeInitialized = true;
        private readonly object _lock = new object();
        private readonly ILog _log;
        private readonly IBlockchainApiClientProvider _blockchainApiClientProvider;
        private readonly ConcurrentDictionary<string, bool> _cacheCapabilities;
        private readonly int _apiCallRetryDelay = 5;
        private readonly ConcurrentDictionary<string, int> _blockchainConnectAttemptsDelays;
        private List<Task> _tasks = null;

        public BlockchainWalletsCacheService(IBlockchainApiClientProvider blockchainApiClientProvider, ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _blockchainApiClientProvider = blockchainApiClientProvider;
            _cacheCapabilities = new ConcurrentDictionary<string, bool>();
            _blockchainConnectAttemptsDelays = new ConcurrentDictionary<string, int>();
        }

        public void FireInitializationAndForget()
        {
            lock (_lock)
            {
                if (!_canBeInitialized)
                {
                    throw new InvalidOperationException("Can be initialized only once.");
                }

                _canBeInitialized = false;
            }

            var apis = _blockchainApiClientProvider.GetApiClientsEnumerable();
            _tasks = new List<Task>(apis.Count());
            apis.ForEach(x =>
            {
                var task = new Task(async () => { await InitializeOneAsync(x.Key, x.Value); });
                _tasks.Add(task);
                task.Start();
            });
        }

        public bool IsPublicExtensionRequired(string blockchainType)
        {
            var key = GetKey(blockchainType);
            if (!_cacheCapabilities.TryGetValue(key, out var isPublicExtensionRequired))
            {
                throw new InvalidOperationException($"There is no cache for {blockchainType} yet");
            }

            return isPublicExtensionRequired;
        }

        private async Task InitializeOneAsync(string blockchainType, IBlockchainApiClient apiClient)
        {
            while (true)
            {
                try
                {
                    // Capabilities
                    var capabilities = await apiClient.GetCapabilitiesAsync();

                    var key = GetKey(blockchainType);
                    _cacheCapabilities.TryAdd(key, capabilities.IsPublicAddressExtensionRequired);

                    // Exit on success
                    if (_blockchainConnectAttemptsDelays.ContainsKey(blockchainType))
                        _blockchainConnectAttemptsDelays.Remove(blockchainType, out var _);

                    break;
                }
                catch (Exception ex)
                {
                    _log.Warning($"Unable to obtain and/or store in cache the capabilities or constants data for the blockchain type {blockchainType}. Will retry till success.", ex);

                    if (!_blockchainConnectAttemptsDelays.TryGetValue(blockchainType, out var delay))
                    {
                        delay = _apiCallRetryDelay;
                        _blockchainConnectAttemptsDelays.TryAdd(blockchainType, delay);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(delay));

                    _blockchainConnectAttemptsDelays[blockchainType] = Math.Min(delay * 2, 600);
                }
            }
        }

        private static string GetKey(string blockchainType)
        {
            return $"{blockchainType}-IsPublicAddressExtensionRequired";
        }
    }
}
