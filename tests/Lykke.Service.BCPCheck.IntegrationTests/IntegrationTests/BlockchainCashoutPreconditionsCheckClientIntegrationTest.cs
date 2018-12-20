using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses;
using Lykke.Service.BlockchainWallets.CTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.BCPCheck.IntegrationTests.IntegrationTests
{
    public class BlockchainCashoutPreconditionsCheckClientIntegrationTest : BaseTest
    {
        private const string _stellarBlockchainType = "Stellar";
        private const string _etcBlockchainType = "EthereumClassic";
        private const string _etcAssetId = "915c4074-ec20-40ed-b8b7-5e3cc2f303b1";
        private const string _stellarAssetId = "b5a0389c-fe57-425f-ab17-af41638f6b89";
        private readonly Guid _clientId = Guid.Parse("5b39a8a8-af3f-451d-8284-3c06980e2435");

        public BlockchainCashoutPreconditionsCheckClientIntegrationTest(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        #region ValidityCheck

        [Theory]
        [InlineData(_etcAssetId, "0x81b7E08F65Bdf5648606c89998A9CC8164397647", true)]
        [InlineData(_etcAssetId, "0x406561f72e25ab41200fa3d52badc5a21", false)]
        [InlineData(_stellarAssetId, "GDF4MNKB57VPSF2ZAM36YEXH6TFEXQGQT4IJVR3IOMZQIFC2B44Z4HBL", true)]
        [InlineData(_stellarAssetId, "GDF4MNKB57VPSF2ZAM36YEXH6TFEXQGQT4IJVR3IOMZQIFC2B44Z4HBL$gmp91dzbofqrmxdw4hqt4idwyw", false)]
        [InlineData("d1a7ffea-2ca1-48b6-a41f-a7058ddb0dfa", "lykkedev$0sdfsdf$", false)]
        [InlineData("d1a7ffea-2ca1-48b6-a41f-a7058ddb0dfa", "lykkedev$$$WHY$$$", false)]
        [InlineData("2c2c94f9-8fff-4307-89c6-8f5f5f586724", "lykkedev2018:123::::", false)]
        //[InlineData("463b1b32-b801-4ea9-a321-7e81bb73d947", "rwN1jdjVQdpMPa8TYqqCHHKhbXcmtTae83+123", true)]
        public async Task ValidateCashoutAsync_ExecuteOnDataSet(string assetId, string destinationAddress, bool isValidExpected)
        {
            var blockchainCashoutPreconditionsCheckClient = GenerateBlockchainWalletsClient();

            (bool isValidActual, IEnumerable<ValidationErrorResponse> errors) = await
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
            string blockedAddress = "GA4FSFQNHZ5A7VVRDC3UKLUVHX7BAZGRZ432XOAFVQJMYULPKKPPE7PY";
            var newBlackList = new AddBlackListModel()
            {
                BlockchainType = _stellarBlockchainType,
                BlockedAddress = blockedAddress,
                IsCaseSensitive = false
            };

            await blockchainCashoutPreconditionsCheckClient.AddToBlackListAsync(newBlackList);
            var blackList = await blockchainCashoutPreconditionsCheckClient.GetBlackListAsync(_stellarBlockchainType, blockedAddress);
            var allEtcBlackLists = await blockchainCashoutPreconditionsCheckClient.GetAllBlackListsAsync(_stellarBlockchainType, 500, null);
            await blockchainCashoutPreconditionsCheckClient.DeleteBlackListAsync(_stellarBlockchainType, blockedAddress);
            var blackListAfterDeletion = await blockchainCashoutPreconditionsCheckClient.GetBlackListAsync(_stellarBlockchainType, blockedAddress);
            var allEtcBlackListsAfterDeletion = await blockchainCashoutPreconditionsCheckClient.GetAllBlackListsAsync(_stellarBlockchainType, 500, null);

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
