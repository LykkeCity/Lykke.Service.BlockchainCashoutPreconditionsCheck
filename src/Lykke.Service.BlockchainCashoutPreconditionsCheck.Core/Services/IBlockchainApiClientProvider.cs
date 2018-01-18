using Lykke.Service.BlockchainApi.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IBlockchainApiClientProvider
    {
        IBlockchainApiClient Get(string blockchainType);
    }
}
