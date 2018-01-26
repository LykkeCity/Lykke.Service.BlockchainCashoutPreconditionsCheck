using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
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

        public ValidationService(IBlockchainApiClientProvider blockchainApiClientProvider, Lykke.Service.Assets.Client.IAssetsService assetsService)
        {
            _blockchainApiClientProvider = blockchainApiClientProvider;
            _assetsService = assetsService;
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

            var asset = await _assetsService.AssetGetAsync(cashoutModel.AssetId);
            List<ValidationError> errors = new List<ValidationError>(1);

            if (asset == null)
                throw new ArgumentValidationException($"Asset with Id-{cashoutModel.AssetId} does not exists", "AssetId");

            if (string.IsNullOrEmpty(asset.BlockchainIntegrationLayerId))
                throw new ArgumentValidationException($"Given asset Id-{cashoutModel.AssetId} is not a part of Blockchain Integration Layer", "AssetId");

            var blockchainClient = _blockchainApiClientProvider.Get(asset.BlockchainIntegrationLayerId);

            //Check volume in the future if needed
            if (string.IsNullOrEmpty(cashoutModel.DestinationAddress) || 
                !await blockchainClient.IsAddressValidAsync(cashoutModel.DestinationAddress))
            {
                errors.Add(ValidationError.Create(ValidationErrorType.AddressIsNotValid, "Address is not valid"));
            }

            return errors;
        }

        private IEnumerable<ValidationError> FieldNotValidResult(string message)
        {
            return new[] { ValidationError.Create(ValidationErrorType.FieldNotValid, message) };
        }
    }
}
