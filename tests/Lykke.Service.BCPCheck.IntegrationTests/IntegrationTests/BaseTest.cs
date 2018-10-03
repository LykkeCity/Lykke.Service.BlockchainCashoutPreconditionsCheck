using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client;
using Lykke.Service.BlockchainWallets.Client;
using Lykke.Service.BlockchainWallets.Client.ClientGenerator;
using Lykke.Service.BlockchainWallets.Client.DelegatingMessageHandlers;
using Lykke.Service.BlockchainWallets.CTests.Common;
using Moq;
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
            var log = new Mock<ILog>();
            var logFactory = new Mock<ILogFactory>();
            logFactory.Setup(x => x.CreateLog(It.IsAny<object>())).Returns(log.Object);

            //var factory = new WebApplicationFactory<Startup>();
            var testClient = _factory.CreateClient();
            //interceptor redirects request to the TEST Server.
            var interceptor = new RequestInterceptorHandler(testClient);
            var blockchainWalletClient =
                new BlockchainCashoutPreconditionsCheckClient("http://localhost:5009",
                    logFactory.Object,
                    interceptor);

            return blockchainWalletClient;
        }
    }
}
