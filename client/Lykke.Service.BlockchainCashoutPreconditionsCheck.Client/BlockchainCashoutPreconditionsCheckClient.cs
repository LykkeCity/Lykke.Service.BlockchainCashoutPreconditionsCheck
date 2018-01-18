using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models;
using MoreLinq;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public class BlockchainCashoutPreconditionsCheckClient : IBlockchainCashoutPreconditionsCheckClient, IDisposable
    {
        private readonly ILog _log;
        private IBlockchainCashoutPreconditionsCheckAPI _service;

        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl, ILog log)
        {
            _log = log;
            _service = new BlockchainCashoutPreconditionsCheckAPI(new Uri(serviceUrl));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validateCashoutModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<(bool isAllowed, IEnumerable<ValidationErrorResponse>)> ValidateCashoutAsync(CashoutValidateModel validateCashoutModel)
        {
            bool isAllowed = false;
            IEnumerable<ValidationErrorResponse> validationErrors;
            var response =  await _service.CheckWithHttpMessagesAsync(validateCashoutModel.AssetId, validateCashoutModel.Amount, validateCashoutModel.DestinationAddress);
            var responseObject = response.Body;
            switch (responseObject)
            {
                case CashoutValidityResult validationErrorResponse:
                    isAllowed = validationErrorResponse.IsAllowed;
                    validationErrors = validationErrorResponse.ValidationErrors;
                    break;
                case ErrorResponse errorResponse:
                    validationErrors = new[] { new ValidationErrorResponse(ValidationErrorType.None, errorResponse.ErrorMessage) };
                    break;
                default:
                    throw new Exception($"Unknown response: {responseObject.ToJson()}");
            }

            return (isAllowed, validationErrors);
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            _service.Dispose();
            _service = null;
        }
    }
}
