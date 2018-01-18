using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Health;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class BlockchainApiClientProvider : IBlockchainApiClientProvider
    {
        private readonly IIndex<string, IBlockchainApiClient> _clients;

        public BlockchainApiClientProvider(IIndex<string, IBlockchainApiClient> clients)
        {
            _clients = clients;
        }

        public IBlockchainApiClient Get(string blockchainType)
        {
            if (!_clients.TryGetValue(blockchainType, out var client))
            {
                throw new InvalidOperationException($"Blockchain API client [{blockchainType}] is not found");
            }

            return client;
        }
    }
}
