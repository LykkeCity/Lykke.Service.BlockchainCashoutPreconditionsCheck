using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IBlockchainSettingsProvider
    {
        BlockchainSettings Get(string blockchainType);
    }
}
