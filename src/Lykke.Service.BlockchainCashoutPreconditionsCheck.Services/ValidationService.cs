using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Health;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IBlockchainApiClientProvider _blockchainApiClientProvider;
        private readonly IAssetsService _assetsService;
        private readonly IBlackListService _blackListService;

        public ValidationService(IBlockchainApiClientProvider blockchainApiClientProvider, 
            Lykke.Service.Assets.Client.IAssetsService assetsService,
            IBlackListService blackListService)
        {
            _blockchainApiClientProvider = blockchainApiClientProvider;
            _assetsService = assetsService;
            _blackListService = blackListService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cashoutModel"></param>
        /// <returns>
        /// ValidationError - client error
        /// ArgumentValidationException - developer error
        /// </returns>
        public async Task<IEnumerable<ValidationError>> ValidateAsync(CashoutModel cashoutModel)
        {
            if (cashoutModel == null)
                return FieldNotValidResult("cashoutModel can't be null");

            if (string.IsNullOrEmpty(cashoutModel.AssetId))
                return FieldNotValidResult("cashoutModel.AssetId can't be null or empty");

            Asset asset = null;
            try
            {
                asset = await _assetsService.AssetGetAsync(cashoutModel.AssetId);
            }
            catch (Exception e)
            {
                throw new ArgumentValidationException($"Asset with Id-{cashoutModel.AssetId} does not exists", "assetId");
            }

            if (asset == null)
                throw new ArgumentValidationException($"Asset with Id-{cashoutModel.AssetId} does not exists", "assetId");

            if (string.IsNullOrEmpty(asset.BlockchainIntegrationLayerId))
                throw new ArgumentValidationException($"Given asset Id-{cashoutModel.AssetId} is not a part of Blockchain Integration Layer", "assetId");

            var blockchainClient = _blockchainApiClientProvider.Get(asset.BlockchainIntegrationLayerId);

            List<ValidationError> errors = new List<ValidationError>(1);

            var isBlocked = await _blackListService.IsBlockedAsync(asset.BlockchainIntegrationLayerId,
                cashoutModel.DestinationAddress);

            if (isBlocked)
            {
                errors.Add(ValidationError.Create(ValidationErrorType.BlackListedAddress, "Address is in the black list"));
            }

            if (string.IsNullOrEmpty(cashoutModel.DestinationAddress) ||
                !await blockchainClient.IsAddressValidAsync(cashoutModel.DestinationAddress))
            {
                errors.Add(ValidationError.Create(ValidationErrorType.AddressIsNotValid, "Address is not valid"));
            }

            if (cashoutModel.Volume != 0 && 
                Math.Abs(cashoutModel.Volume) < (decimal)asset.CashoutMinimalAmount)
            {
                var minimalAmount = asset.CashoutMinimalAmount.GetFixedAsString(asset.Accuracy).TrimEnd('0');

                errors.Add(ValidationError.Create(ValidationErrorType.LessThanMinCashout, minimalAmount));
            }

            return errors;
        }

        private IEnumerable<ValidationError> FieldNotValidResult(string message)
        {
            return new[] { ValidationError.Create(ValidationErrorType.FieldNotValid, message) };
        }
    }
}
