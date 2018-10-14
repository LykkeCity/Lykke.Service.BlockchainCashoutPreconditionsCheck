using Lykke.HttpClientGenerator.Caching;
using System.Net.Http;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.ClientGenerator;

namespace Lykke.Service.BlockchainWallets.Client.ClientGenerator
{
    public interface IBlockchainCashoutPreconditionsApiFactory
    {
        IBlockchainCashoutPreconditionsCheckApi CreateNew(BlockchainCashoutPreconditionsClientSettings settings,
            bool withCaching = false,
            IClientCacheManager clientCacheManager = null,
            params DelegatingHandler[] handlers);

        IBlockchainCashoutPreconditionsCheckApi CreateNew(string url, bool withCaching = false,
            IClientCacheManager clientCacheManager = null, params DelegatingHandler[] handlers);
    }
}
