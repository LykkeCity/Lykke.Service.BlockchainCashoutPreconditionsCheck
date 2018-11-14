using Lykke.Service.BCPCheck.IntegrationTests.DelegatingHandlers;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client;
using Lykke.Service.BlockchainWallets.CTests.Common;
using Xunit;

namespace Lykke.Service.BCPCheck.IntegrationTests.IntegrationTests
{
    public class BaseTest :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;

        public BaseTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        protected BlockchainCashoutPreconditionsCheckClient GenerateBlockchainWalletsClient()
        {
            //var factory = new WebApplicationFactory<Startup>();
            var testClient = _factory.CreateClient();
            //interceptor redirects request to the TEST Server.
            var interceptor = new RequestInterceptorHandler(testClient);
            var blockchainWalletClient =
                new BlockchainCashoutPreconditionsCheckClient("http://localhost:5009",
                    interceptor);

            return blockchainWalletClient;
        }
    }
}
