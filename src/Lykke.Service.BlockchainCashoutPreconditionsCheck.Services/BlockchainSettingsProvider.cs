using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class BlockchainSettingsProvider : IBlockchainSettingsProvider
    {
        private readonly IImmutableDictionary<string, BlockchainSettings> _settings;

        public BlockchainSettingsProvider(BlockchainsIntegrationSettings settings)
        {
            _settings = settings?.Blockchains.ToImmutableDictionary(x => x.Type, x=> x);
        }

        public BlockchainSettings Get(string blockchainType)
        {
            if (!_settings.TryGetValue(blockchainType, out var result))
            {
                throw new InvalidOperationException($"Blockchain Settings [{blockchainType}] is not found");
            }

            return result;
        }
    }
}
