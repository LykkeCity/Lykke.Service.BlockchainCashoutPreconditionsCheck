using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses;
using Lykke.Service.BlockchainWallets.Client;
using Lykke.Service.BlockchainWallets.Client.ClientGenerator;
using Lykke.Service.BlockchainWallets.Client.DelegatingMessageHandlers;
using Lykke.Service.BlockchainWallets.CTests.Common;
using Moq;
using Xunit;

namespace Lykke.Service.BCPCheck.IntegrationTests.IntegrationTests
{
    public class BlockchainCashoutPreconditionsCheckClientIntegrationTest : BaseTest
    {
        private const string _etcBlockchainType = "EthereumClassic";
        private const string _etcAssetId = "62c04960-4015-4945-bb7e-8e4a193b3653";
        private const string _stellarAssetId = "aa8e5f54-f6a6-4b82-b493-6b098db79c5f";
        private readonly Guid _clientId = Guid.Parse("5b39a8a8-af3f-451d-8284-3c06980e2435");

        public BlockchainCashoutPreconditionsCheckClientIntegrationTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        #region ValidityCheck

        [Theory]
        [InlineData(_etcAssetId, "0x81b7E08F65Bdf5648606c89998A9CC8164397647", true)]
        [InlineData(_etcAssetId, "0x406561f72e25ab41200fa3d52badc5a21", false)]
        [InlineData(_stellarAssetId, "GDF4MNKB57VPSF2ZAM36YEXH6TFEXQGQT4IJVR3IOMZQIFC2B44Z4HBL$gmp91dzbofqrmxdw4hqt4idwyw", false)]
        [InlineData("d1a7ffea-2ca1-48b6-a41f-a7058ddb0dfa", "lykkedev$0sdfsdf$", false)]
        [InlineData("d1a7ffea-2ca1-48b6-a41f-a7058ddb0dfa", "lykkedev$$$WHY$$$", false)]
        [InlineData("2c2c94f9-8fff-4307-89c6-8f5f5f586724", "lykkedev2018:123::::", false)]
        [InlineData("1b542b1e-ffe0-4785-b05b-c43d775cc2d0", "rwN1jdjVQdpMPa8TYqqCHHKhbXcmtTae83+123", true)]
        public async Task ValidateCashoutAsync_ExecuteOnDataSet(string assetId, string destinationAddress, bool isValidExpected)
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();

            (bool isValidActual, IEnumerable<ValidationErrorResponse> errrors) = await
                blockchainCashoutPreconditionsCheckClient.ValidateCashoutAsync(new CheckCashoutValidityModel()
                {
                    ClientId = _clientId,
                    AssetId = assetId,
                    DestinationAddress = destinationAddress,
                    Amount = 0
                });

            Assert.Equal(isValidExpected, isValidActual);
        }

        #endregion ValidityCheck

        #region BlackLists

        [Fact]
        public async Task CheckBlackListPositiveRestFlow_AllOperationsWorkAsExpected()
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();
            string blockedAddress = "0xdc52EBcf077E64076321772ef39eb336a27F8D12";
            var newBlackList = new AddBlackListModel()
            {
                BlockchainType = _etcBlockchainType,
                BlockedAddress = blockedAddress,
                IsCaseSensitive = false
            };

            await blockchainCashoutPreconditionsCheckClient.AddToBlackListAsync(newBlackList);
            var blackList = await blockchainCashoutPreconditionsCheckClient.GetBlackListAsync(_etcBlockchainType, blockedAddress);
            var allEtcBlackLists = await blockchainCashoutPreconditionsCheckClient.GetAllBlackListsAsync(_etcBlockchainType, 500, null);
            await blockchainCashoutPreconditionsCheckClient.DeleteBlackListAsync(_etcBlockchainType, blockedAddress);
            var blackListAfterDeletion = await blockchainCashoutPreconditionsCheckClient.GetBlackListAsync(_etcBlockchainType, blockedAddress);
            var allEtcBlackListsAfterDeletion = await blockchainCashoutPreconditionsCheckClient.GetAllBlackListsAsync(_etcBlockchainType, 500, null);

            Assert.True(blackList != null);
            Assert.True(allEtcBlackLists.List.FirstOrDefault(x => x.BlockedAddress == blockedAddress) != null);
            Assert.True(blackListAfterDeletion == null);
            Assert.True(allEtcBlackListsAfterDeletion.List.FirstOrDefault(x => x.BlockedAddress == blockedAddress) == null);
        }

        [Fact]
        public async Task AddToBlackListAsync_NotValidParameters_ExcetionThrow()
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();
            string blockedAddress = "";
            var newBlackList = new AddBlackListModel()
            {
                BlockchainType = _etcBlockchainType,
                BlockedAddress = blockedAddress,
                IsCaseSensitive = false
            };

            await Assert.ThrowsAsync<BlockchainCashoutPreconditionsCheck.Client.Exceptions.ErrorResponseException>(async () =>
                {
                    await blockchainCashoutPreconditionsCheckClient.AddToBlackListAsync(newBlackList);
                });
        }

        #endregion BlackLists
    }
}
