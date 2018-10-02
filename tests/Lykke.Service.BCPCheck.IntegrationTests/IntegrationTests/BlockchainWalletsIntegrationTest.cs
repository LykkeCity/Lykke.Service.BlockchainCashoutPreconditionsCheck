using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses;
using Lykke.Service.BlockchainWallets.Client;
using Lykke.Service.BlockchainWallets.Client.ClientGenerator;
using Lykke.Service.BlockchainWallets.Client.DelegatingMessageHandlers;
using Lykke.Service.BlockchainWallets.CTests.Common;
using Moq;
using Xunit;

namespace Lykke.Service.BCPCheck.IntegrationTests.IntegrationTests
{
    public class BlockchainWalletsIntegrationTest :
        IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly string _blockchainType = "EthereumClassic";
        private readonly Guid _clientId = Guid.Parse("5b39a8a8-af3f-451d-8284-3c06980e2435");

        public BlockchainWalletsIntegrationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ValidateCashoutAsync_WrongAddress_IsNotValid()
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();

            (bool isAllowed, IEnumerable<ValidationErrorResponse> errrors) = await
                blockchainCashoutPreconditionsCheckClient.ValidateCashoutAsync(new CheckCashoutValidityModel()
                {
                    ClientId = _clientId,
                    AssetId = "62c04960-4015-4945-bb7e-8e4a193b3653",
                    DestinationAddress = "0x406561f72e25ab41200fa3d52badc5a21",
                    Amount = 0
                });

            Assert.False(isAllowed);
        }

        [Fact]
        public async Task ValidateCashoutAsync_RightAddress_IsValid()
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();

            (bool isAllowed, IEnumerable<ValidationErrorResponse> errrors) = await
                blockchainCashoutPreconditionsCheckClient.ValidateCashoutAsync(new CheckCashoutValidityModel()
                {
                    ClientId = _clientId,
                    AssetId = "62c04960-4015-4945-bb7e-8e4a193b3653",
                    DestinationAddress = "0x81b7E08F65Bdf5648606c89998A9CC8164397647",
                    Amount = 0
                });

            Assert.True(isAllowed);
        }

        private BlockchainCashoutPreconditionsCheckClient GenerateBlockchainWalletsClient()
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
