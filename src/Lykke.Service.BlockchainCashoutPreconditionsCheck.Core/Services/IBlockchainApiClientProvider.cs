using System.Collections.Generic;
using Lykke.Service.BlockchainApi.Client;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IBlockchainApiClientProvider
    {
        IBlockchainApiClient Get(string blockchainType);

        IEnumerable<KeyValuePair<string, IBlockchainApiClient>> GetApiClientsEnumerable();
    }
}
