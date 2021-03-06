﻿using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<bool> IsBlockedWithoutAddressValidationAsync(string blockchainType, string blockedAddress)
        {
            await ThrowOnNotSupportedBlockchainType(blockchainType);

            var blackList = await _blackListRepository.TryGetAsync(blockchainType, blockedAddress);

            if (blackList == null)
            {
                return false;
            }

            var isBlocked = IsBlocked(blockedAddress, blackList);

            return isBlocked;
        }

        public async Task<bool> IsBlockedAsync(string blockchainType, string blockedAddress)
        {
            await ThrowOnNotSupportedBlockchainType(blockchainType, blockedAddress);

            var blackList = await _blackListRepository.TryGetAsync(blockchainType, blockedAddress);

            if (blackList == null)
            {
                return false;
            }

            var isBlocked = IsBlocked(blockedAddress, blackList);

            return isBlocked;
        }

        public async Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress)
        {
            await ThrowOnNotSupportedBlockchainType(blockchainType, blockedAddress);

            var model = await _blackListRepository.TryGetAsync(blockchainType, blockedAddress);

            return model;
        }

        public async Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take, string continuationToken = null)
        {
            await ThrowOnNotSupportedBlockchainType(blockchainType);

            var (models, newToken) = await _blackListRepository.TryGetAllAsync(blockchainType, take, continuationToken);

            return (models, newToken);
        }

        public async Task SaveAsync(BlackListModel model)
        {
            await ThrowOnNotSupportedBlockchainType(model?.BlockchainType ?? "", model?.BlockedAddress);

            await _blackListRepository.SaveAsync(model);
        }

        public async Task DeleteAsync(string blockchainType, string blockedAddress)
        {
            await ThrowOnNotSupportedBlockchainType(blockchainType, blockedAddress);

            await _blackListRepository.DeleteAsync(blockchainType, blockedAddress);
        }

        /// <exception cref="ArgumentValidationException"></exception>
        private Task<IBlockchainApiClient> ThrowOnNotSupportedBlockchainType(string blockchainType)
        {
            IBlockchainApiClient blockchainClient;

            try
            {
                blockchainClient = _blockchainApiClientProvider.Get(blockchainType); //throws
            }
            catch (ArgumentValidationException)
            {
                throw new ArgumentValidationException($"{blockchainType} is not a valid type", "blockchainType");
            }
            

            return Task.FromResult(blockchainClient);
        }

        private async Task ThrowOnNotSupportedBlockchainType(string blockchainType, string blockedAddress)
        {
            var blockchainClient = await ThrowOnNotSupportedBlockchainType(blockchainType);//throws
            if (string.IsNullOrEmpty(blockedAddress) ||
                !await blockchainClient.IsAddressValidAsync(blockedAddress))
            {
                throw new ArgumentValidationException($"{blockedAddress} is not a valid address for {blockchainType}", "blockedAddress");
            }
        }

        private static bool IsBlocked(string blockedAddress, BlackListModel blackList)
        {
            var isBlocked = blackList.IsCaseSensitive && blockedAddress == blackList.BlockedAddress ||
                            !blackList.IsCaseSensitive && blockedAddress.ToLower() == blackList.BlockedAddressLowCase;
            return isBlocked;
        }
    }
}
