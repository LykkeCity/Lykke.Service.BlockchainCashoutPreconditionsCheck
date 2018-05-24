using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lykke.Service.BCPCheck.Tests
{
    //Naming: MethodName__TestCase__ExpectedResult
    [TestClass]
    public class BlackListServiceTest
    {
        private const string BlockchainType = "EthereumClassic";
        private const string BlockedAddressValid = "0xeb574cD5A407Fefa5610fCde6Aec13D983BA527c";
        private const string BlockedAddressNotValid = "12323545tsrdfe54rydfgryg65uhjdfesd";
        private BlackListService _logic;

        [TestInitialize]
        public void Init()
        {
            var repo = new BlackListRepositoryFake();
            var blockchainApiClientProviderMock = new Mock<IBlockchainApiClientProvider>();
            var blockchainApiClient = new Mock<IBlockchainApiClient>();
            blockchainApiClient.Setup(x => x.IsAddressValidAsync(BlockedAddressValid)).Returns(Task.FromResult(true));
            blockchainApiClient.Setup(x => x.IsAddressValidAsync(BlockedAddressValid.ToLower())).Returns(Task.FromResult(true));

            for (int i = 0; i < 9; i++)
            {
                var z = i;
                blockchainApiClient.Setup(x => x.IsAddressValidAsync($"0x{z}...")).Returns(Task.FromResult(true));
            }

            blockchainApiClientProviderMock.Setup(x => x.Get(It.IsAny<string>())).Returns(blockchainApiClient.Object);

            _logic = new BlackListService(repo, blockchainApiClientProviderMock.Object);
        }

        [TestMethod]
        public void IsBlockedAsync__AddedLowerCaseSensitiveCheck__False()
        {
            var model = SaveBlackListModel(BlockedAddressValid.ToLower(), true);
            var result = _logic.IsBlockedAsync(model.BlockchainType, BlockedAddressValid).Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsBlockedAsync__AddedLowerCaseNotSensitiveCheck__True()
        {
            var model = SaveBlackListModel(BlockedAddressValid.ToLower(), false);
            var result = _logic.IsBlockedAsync(model.BlockchainType, BlockedAddressValid).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsBlockedAsync__AddedNormalCaseSensitiveCheck__True()
        {
            var model = SaveBlackListModel(BlockedAddressValid, true);
            var result = _logic.IsBlockedAsync(model.BlockchainType, BlockedAddressValid).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsBlockedAsync__AddedNormalCaseNotSensitiveCheck__True()
        {
            var model = SaveBlackListModel(BlockedAddressValid, false);
            var result = _logic.IsBlockedAsync(model.BlockchainType, BlockedAddressValid).Result;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsBlockedAsync__NotBlocked__False()
        {
            var result = _logic.IsBlockedAsync(BlockchainType, BlockedAddressValid).Result;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryGetAsync__NotYetAdded__IsNull()
        {
            var blocked = _logic.TryGetAsync(BlockchainType, BlockedAddressValid).Result;

            Assert.IsNull(blocked);
        }

        [TestMethod]
        public void DeleteAsync__NotYetAdded__NoException()
        {
            _logic.DeleteAsync(BlockchainType, BlockedAddressValid).Wait();
        }

        [TestMethod]
        public void DeleteAsync__AddedBefore__Removed()
        {
            SaveBlackListModel(BlockedAddressValid, false);
            _logic.DeleteAsync(BlockchainType, BlockedAddressValid).Wait();

            var deleted = _logic.TryGetAsync(BlockchainType, BlockedAddressValid).Result;

            Assert.IsNull(deleted);
        }

        [TestMethod]
        public void SaveAsync__NotYetAdded__IsAdded()
        {
            AddBlackListAndCheck();
        }

        [TestMethod]
        public void TryGetAllAsync__NotYetAdded__NoException()
        {
            var result = _logic.TryGetAllAsync(BlockchainType, 100, null).Result;

            Assert.AreEqual(result.Item1.Count(), 0);
            Assert.AreEqual(result.continuationToken, null);
        }

        [TestMethod]
        public void TryGetAllAsync__AddedThree__RetreiveAll()
        {
            int count = 9;
            for (int i = 0; i < count; i++)
            {
                SaveBlackListModel($"0x{i}...", false);
            }
            
            var result = _logic.TryGetAllAsync(BlockchainType, count, null).Result;

            Assert.AreEqual(result.Item1.Count(), count);
            Assert.AreEqual(result.continuationToken, null);
        }

        [TestMethod]
        public void SaveAsync__AlreadyAdded__IsUpdated()
        {
            AddBlackListAndCheck();
            AddBlackListAndCheck();
        }

        #region Private

        private void AddBlackListAndCheck()
        {
            var model = SaveBlackListModel(BlockedAddressValid, true);

            var added = _logic.TryGetAsync(BlockchainType, BlockedAddressValid).Result;

            Assert.AreEqual(model.BlockedAddress, added.BlockedAddress);
             Assert.AreEqual(model.BlockchainType, added.BlockchainType);
            Assert.AreEqual(model.BlockedAddressLowCase, added.BlockedAddressLowCase);
            Assert.AreEqual(model.IsCaseSensitive, added.IsCaseSensitive);
        }

        private BlackListModel SaveBlackListModel(string blockedAddress, bool isCaseSensitiv)
        {
            var model = new BlackListModel(BlockchainType, blockedAddress, isCaseSensitiv);

            _logic.SaveAsync(model).Wait();
            return model;
        }

        #endregion
    }
}
