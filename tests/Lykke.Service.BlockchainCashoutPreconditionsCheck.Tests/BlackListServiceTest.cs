using System;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Tests
{
    //Naming: MethodName__TestCase__ExpectedResult
    [TestClass]
    public class BlackListServiceTest
    {
        private BlackListService _logic;

        [TestInitialize]
        public void Init()
        {
            var repo = new BlackListRepositoryFake();
            var blockchainApiClientProviderMock = new Mock<IBlockchainApiClientProvider>();
            var blockchainApiClient = new Mock<IBlockchainApiClient>();
            blockchainApiClientProviderMock.Setup(x => x.Get(It.IsAny<string>())).Returns(blockchainApiClient.Object);

            _logic = new BlackListService(repo, blockchainApiClientProviderMock.Object);
        }

        [TestMethod]
        public void IsBlockedAsync__NotBlocked__False()
        {
            var result = _logic.IsBlockedAsync("EthereumClassic", "0x000000000...").Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryGetAsyncc__NotYetAdded__IsNull()
        {
            var blocked = _logic.TryGetAsync("EthereumClassic", "0x000000000...").Result;

            Assert.IsNull(blocked);
        }
    }
}
