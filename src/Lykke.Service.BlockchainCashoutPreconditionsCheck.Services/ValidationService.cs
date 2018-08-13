using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainWallets.Client;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IBlockchainApiClientProvider _blockchainApiClientProvider;
        private readonly IAssetsService _assetsService;
        private readonly IBlockchainSettingsProvider _blockchainSettingsProvider;
        private readonly IBlackListService _blackListService;
        private readonly IBlockchainWalletsClient _blockchainWalletsClient;

        public ValidationService(IBlockchainApiClientProvider blockchainApiClientProvider,
            IAssetsService assetsService,
            IBlockchainSettingsProvider blockchainSettingsProvider,
            IBlockchainWalletsClient blockchainWalletsClient,
            IBlackListService blackListService)
        {
            _blockchainApiClientProvider = blockchainApiClientProvider;
            _assetsService = assetsService;
            _blockchainSettingsProvider = blockchainSettingsProvider;
            _blackListService = blackListService;
            _blockchainWalletsClient = blockchainWalletsClient;
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

            if (cashoutModel.Volume > 0)
            {
                if (cashoutModel.Volume != 0 &&
                    Math.Abs(cashoutModel.Volume) < (decimal) asset.CashoutMinimalAmount)
                {
                    var minimalAmount = asset.CashoutMinimalAmount.GetFixedAsString(asset.Accuracy).TrimEnd('0');

                    errors.Add(ValidationError.Create(ValidationErrorType.LessThanMinCashout, minimalAmount));
                }
            }
            else
            {
                errors.Add(ValidationError.Create(ValidationErrorType.LessThanMinCashout,
                    "Amount should be positive number"));
            }

            var blockchainSettings = _blockchainSettingsProvider.Get(asset.BlockchainIntegrationLayerId);

            if (cashoutModel.DestinationAddress == blockchainSettings.HotWalletAddress)
            {
                errors.Add(ValidationError.Create(ValidationErrorType.HotwalletTargetProhibited, "Hot wallet as destitnation address prohibited"));
            }

            var capabilities = await _blockchainWalletsClient.GetCapabilititesAsync(asset.BlockchainIntegrationLayerId);
            if (capabilities.IsPublicAddressExtensionRequired)
            {

                var hotWalletParseResult = await _blockchainWalletsClient.ParseAddressAsync(asset.BlockchainIntegrationLayerId,
                        blockchainSettings.HotWalletAddress);

                var destAddressParseResult = await _blockchainWalletsClient.ParseAddressAsync(asset.BlockchainIntegrationLayerId,
                        cashoutModel.DestinationAddress);

                if (hotWalletParseResult.BaseAddress == destAddressParseResult.BaseAddress)
                {
                    var existedClientIdAsDestination = await _blockchainWalletsClient.TryGetClientIdAsync(asset.BlockchainIntegrationLayerId,
                         asset.BlockchainIntegrationLayerAssetId,
                         cashoutModel.DestinationAddress);

                    if (existedClientIdAsDestination == null)
                    {
                        errors.Add(ValidationError.Create(ValidationErrorType.DepositAddressNotFound, $"Deposit address {cashoutModel.DestinationAddress} not found"));
                    }
                }

                if (!string.IsNullOrEmpty(destAddressParseResult.BaseAddress))
                {
                    if (!(cashoutModel?.DestinationAddress.Contains(destAddressParseResult.BaseAddress) ?? false))
                    {
                        errors.Add(ValidationError.Create(ValidationErrorType.FieldIsNotValid, "Base Address should be part of destination address"));
                    }

                    var isBlockedBase = await _blackListService.IsBlockedAsync(asset.BlockchainIntegrationLayerId,
                        destAddressParseResult.BaseAddress);

                    if (isBlockedBase)
                        errors.Add(ValidationError.Create(ValidationErrorType.BlackListedAddress, "Base Address is in the black list"));
                }
            }

            if (cashoutModel.ClientId != null)
            {
                var clientAddress = await _blockchainWalletsClient.TryGetAddressAsync(asset.BlockchainIntegrationLayerId,
                    asset.BlockchainIntegrationLayerAssetId,
                    cashoutModel.ClientId.Value);

                if (string.Equals(clientAddress?.Address, cashoutModel.DestinationAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    errors.Add(ValidationError.Create(ValidationErrorType.CashoutToSelfAddress, "Withdrawals to the deposit wallet owned by the customer himself prohibited"));
                }

            }

            return errors;
        }

        private IEnumerable<ValidationError> FieldNotValidResult(string message)
        {
            return new[] { ValidationError.Create(ValidationErrorType.FieldIsNotValid, message) };
        }
    }
}
