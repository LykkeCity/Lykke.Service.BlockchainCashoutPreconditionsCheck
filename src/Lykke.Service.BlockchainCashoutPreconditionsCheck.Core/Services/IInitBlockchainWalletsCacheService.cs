using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IInitBlockchainWalletsCacheService
    {
        void FireInitializationAndForget();
    }
}
