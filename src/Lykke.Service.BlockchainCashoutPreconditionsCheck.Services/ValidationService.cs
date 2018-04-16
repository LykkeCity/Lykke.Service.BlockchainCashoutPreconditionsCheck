using System;
using System.Collections.Generic;
using System.Net;
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
using Lykke.Service.BlockchainSignFacade.Client;
using Lykke.Service.BlockchainSignFacade.Contract.Models;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IBlockchainApiClientProvider _blockchainApiClientProvider;
        private readonly IAssetsService _assetsService;
        private readonly IAddressParser _addressParser;
        private readonly IBlockchainSettingsProvider _blockchainSettingsProvider;
        private readonly IBlockchainSignFacadeClient _blockchainSignFacadeClient;

        public ValidationService(IBlockchainApiClientProvider blockchainApiClientProvider, 
            IAssetsService assetsService, 
            IAddressParser addressParser, 
            IBlockchainSettingsProvider blockchainSettingsProvider, 
            IBlockchainSignFacadeClient blockchainSignFacadeClient)
        {
            _blockchainApiClientProvider = blockchainApiClientProvider;
            _assetsService = assetsService;
            _addressParser = addressParser;
            _blockchainSettingsProvider = blockchainSettingsProvider;
            _blockchainSignFacadeClient = blockchainSignFacadeClient;
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
                throw new ArgumentValidationException($"Asset with Id-{cashoutModel.AssetId} does not exists", "AssetId");
            }

            if (asset == null)
                throw new ArgumentValidationException($"Asset with Id-{cashoutModel.AssetId} does not exists", "AssetId");

            if (string.IsNullOrEmpty(asset.BlockchainIntegrationLayerId))
                throw new ArgumentValidationException($"Given asset Id-{cashoutModel.AssetId} is not a part of Blockchain Integration Layer", "AssetId");

            var blockchainClient = _blockchainApiClientProvider.Get(asset.BlockchainIntegrationLayerId);

            List<ValidationError> errors = new List<ValidationError>(1);

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

            var blockchainSettings = _blockchainSettingsProvider.Get(asset.BlockchainIntegrationLayerId);

            if (cashoutModel.DestinationAddress == blockchainSettings.HotWalletAddress)
            {
                errors.Add(ValidationError.Create(ValidationErrorType.HotwalletTargetProhibited, "Hot wallet as destitnation address prohibited"));
            }

            if (blockchainSettings.SupportAddressParts)
            {
                var hotWalletBaseAddress = _addressParser
                    .ParseAddress(blockchainSettings.HotWalletAddress, blockchainSettings.AddressPartsExtractingRegex)
                    .BaseAddress;

                var destinatinationBaseAddress = _addressParser
                    .ParseAddress(cashoutModel.DestinationAddress, blockchainSettings.AddressPartsExtractingRegex)
                    .BaseAddress;

                if (hotWalletBaseAddress == destinatinationBaseAddress)
                {
                    WalletResponse existedInnerAddress = null;
                    try
                    {
                        
                        existedInnerAddress = await _blockchainSignFacadeClient.GetWalletByPublicAddressAsync(asset.BlockchainIntegrationLayerId,
                            cashoutModel.DestinationAddress);
                    }
                    catch (Exception e) when((e.InnerException as Refit.ApiException)?.StatusCode == HttpStatusCode.NotFound)
                    {

                    }
                  

                    if (existedInnerAddress == null)
                    {
                        errors.Add(ValidationError.Create(ValidationErrorType.InnerAddressNotFound, $"Inner address {cashoutModel.DestinationAddress} not found"));
                    }
                }
            }

            return errors;
        }

        private IEnumerable<ValidationError> FieldNotValidResult(string message)
        {
            return new[] { ValidationError.Create(ValidationErrorType.FieldNotValid, message) };
        }
    }
}
