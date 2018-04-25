using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class BlackListService : IBlackListService
    {
        private readonly IBlackListRepository _blackListRepository;
        private readonly IBlockchainApiClientProvider _blockchainApiClientProvider;

        public BlackListService(IBlackListRepository blackListRepository,
            IBlockchainApiClientProvider blockchainApiClientProvider)
        {
            _blackListRepository = blackListRepository;
            _blockchainApiClientProvider = blockchainApiClientProvider;
        }

        public async Task<bool> IsBlockedAsync(string blockchainType, string blockedAddress)
        {
            ThrowOnNotSupportedBlockchainType(blockchainType);

            var blackList = await _blackListRepository.TryGetAsync(blockchainType, blockedAddress);

            if (blackList == null)
            {
                return false;
            }

            bool isBlocked = false;

            isBlocked = (blackList.IsCaseSensitive && blockedAddress == blackList.BlockedAddress) ||
                        (!blackList.IsCaseSensitive && blockedAddress.ToLower() == blackList.BlockedAddressLowCase);

            return isBlocked;
        }

        public async Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress)
        {
            ThrowOnNotSupportedBlockchainType(blockchainType);

            var model = await _blackListRepository.TryGetAsync(blockchainType, blockedAddress);

            return model;
        }

        public async Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take, string continuationToken = null)
        {
            ThrowOnNotSupportedBlockchainType(blockchainType);

            if (take == 0)
                take = Int32.MaxValue;

            var (models, newToken) = await _blackListRepository.TryGetAllAsync(blockchainType, take, continuationToken);

            return (models, newToken);
        }

        public async Task SaveAsync(BlackListModel model)
        {
            ThrowOnNotSupportedBlockchainType(model?.BlockchainType ?? "");

            await _blackListRepository.SaveAsync(model);
        }

        public async Task DeleteAsync(string blockchainType, string blockedAddress)
        {
            ThrowOnNotSupportedBlockchainType(blockchainType);

            await _blackListRepository.DeleteAsync(blockchainType, blockedAddress);
        }

        private void ThrowOnNotSupportedBlockchainType(string blockchainType)
        {
            var blockchainClient = _blockchainApiClientProvider.Get(blockchainType);
        }
    }
}
