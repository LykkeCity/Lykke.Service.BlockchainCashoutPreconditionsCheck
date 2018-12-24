using Lykke.Service.BlockchainApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IBlockchainWalletsCacheService
    {
        bool IsPublicExtensionRequired(string blockchainType);
    }
}
