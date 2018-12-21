using System;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class BlockchainApiClientProvider : IBlockchainApiClientProvider
    {
        private readonly IImmutableDictionary<string, IBlockchainApiClient> _clients;

        public BlockchainApiClientProvider(BlockchainsIntegrationSettings settings, ILogFactory logFactory, int blockchainApiTimeoutSeconds)
        {
            var timeout = TimeSpan.FromSeconds(blockchainApiTimeoutSeconds);
            _clients = settings?.Blockchains.ToImmutableDictionary(x => x.Type, 
                x => (IBlockchainApiClient)new BlockchainApiClient(logFactory, x.ApiUrl, 3, timeout));
        }

        public IBlockchainApiClient Get(string blockchainType)
        {
            if (!_clients.TryGetValue(blockchainType, out var client))
            {
                throw new ArgumentValidationException($"Blockchain API client [{blockchainType}] is not found", "blockchainType");
            }

            return client;
        }

        public IEnumerable<KeyValuePair<string, IBlockchainApiClient>> GetApiClientsEnumerable()
        {
            return _clients.ToImmutableArray();
        }
    }
}
