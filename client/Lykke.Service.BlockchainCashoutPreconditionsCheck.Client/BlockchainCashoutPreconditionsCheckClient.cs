using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.ClientGenerator;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses;
using Lykke.Service.BlockchainWallets.Client.ClientGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Common.Log;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public class BlockchainCashoutPreconditionsCheckClient : IBlockchainCashoutPreconditionsCheckClient, IDisposable
    {
        private readonly ILog _log;
        private IBlockchainCashoutPreconditionsCheckApi _service;

        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl, ILogFactory logFactory, params DelegatingHandler[] handlers)
        {
            var factory = new BlockchainCashoutPreconditionsApiFactory();
            _log = logFactory.CreateLog(this);
            _service = factory.CreateNew(serviceUrl, false, null, handlers);
        }

        /// <summary>
        /// Checks whether or not cashout to the destination address is allowed
        /// </summary>
        /// <param name="validateCashoutModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<(bool isAllowed, IEnumerable<ValidationErrorResponse>)> ValidateCashoutAsync(CheckCashoutValidityModel validateCashoutModel)
        {
            bool isAllowed = false;
            IEnumerable<ValidationErrorResponse> validationErrors;

            try
            {
                var response = await _service.CashoutCheckAsync(validateCashoutModel);

                isAllowed = response.IsAllowed;
                validationErrors = response.ValidationErrors;
            }
            catch (Exception e)
            {
                validationErrors = new[] { ValidationErrorResponse.Create(ValidationErrorType.None, ""/*errorResponse.ErrorMessage*/),  };
            }

            return (isAllowed, validationErrors);
        }

        /// <summary>
        /// Add new address to the specific blockchain type black list
        /// </summary>
        /// <param name="blackListModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task AddToBlackListAsync(AddBlackListModel blackListModel)
        {
            await _service.CreateBlackListAsync(blackListModel);

                //case ErrorResponse errorResponse:
                //    if (!string.IsNullOrEmpty(errorResponse.ErrorMessage))
                //        throw CreateErrorResponseException(errorResponse);
                    
        }

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<BlackListResponse> GetBlackListAsync(string blockchainType, string address)
        {
            var response = await _service.GetBlackListAsync(blockchainType, address);

            return response;
        }

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<BlackListEnumerationResponse> GetAllBlackListsAsync(string blockchainType, int take, string continuationToken = null)
        {
            var response = await _service.GetBlackListsAsync(blockchainType, take, continuationToken);

            return response;
        }

        /// <summary>
        /// Delete address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address to delete from black list</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task DeleteBlackListAsync(string blockchainType, string address)
        {
            await _service.DeleteBlackListAsync(blockchainType, address);
        }

        public void Dispose()
        {
        }

        private ErrorResponseException CreateErrorResponseException(ErrorResponse errorResponse)
        {
            var exception = new ErrorResponseException(errorResponse.ModelErrors);

            return exception;
        }
    }
}
