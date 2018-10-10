using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.ClientGenerator;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Exceptions;
using Lykke.Service.BlockchainWallets.Client.ClientGenerator;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public class BlockchainCashoutPreconditionsCheckClient : IBlockchainCashoutPreconditionsCheckClient, IDisposable
    {
        private IBlockchainCashoutPreconditionsCheckApi _service;

        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl, params DelegatingHandler[] handlers)
        {
            var factory = new BlockchainCashoutPreconditionsApiFactory();
            _service = factory.CreateNew(serviceUrl, false, null, handlers);
        }

        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl) : this(serviceUrl, null)
        {
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
                validateCashoutModel.DestinationAddress = 
                    System.Web.HttpUtility.UrlEncode(validateCashoutModel.DestinationAddress);
                var response = await _service.CashoutCheckAsync(validateCashoutModel);

                isAllowed = response.IsAllowed;
                validationErrors = response.ValidationErrors;
            }
            catch (ApiException e)
            {
                var error = GetErrorResponse(e);
                validationErrors = new[] { ValidationErrorResponse.Create(ValidationErrorType.None, error.Message), };
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
            await ExecuteActionAsync(async () =>
            {
                await _service.CreateBlackListAsync(blackListModel);
            });
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
            return await ExecuteFuncAsync(async () =>
            {
                var response = await _service.GetBlackListAsync(blockchainType, address);

                return response;
            });

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
            return await ExecuteFuncAsync(async () =>
            {
                var response = await _service.GetBlackListsAsync(blockchainType, take, continuationToken);

                return response;
            });
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
            await ExecuteActionAsync(async () =>
            {
                await _service.DeleteBlackListAsync(blockchainType, address);
            });
        }

        public void Dispose()
        {
        }

        private static ErrorResponseException CreateErrorResponseException(ErrorResponse errorResponse)
        {
            var exception = new ErrorResponseException(errorResponse.ModelErrors);

            return exception;
        }

        private async Task ExecuteActionAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Refit.ApiException e)
            {
                var exception = GetErrorResponse(e);
                throw exception;
            }
        }

        private async Task<T> ExecuteFuncAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await func();

                return result;
            }
            catch (Refit.ApiException e)
            {
                var exception = GetErrorResponse(e);
                throw exception;
            }
        }

        private static ErrorResponseException GetErrorResponse(ApiException ex)
        {
            ErrorResponse errorResponse = null;

            try
            {
                errorResponse = ex.GetContentAs<ErrorResponse>();
            }
            catch (Exception e)
            {
                errorResponse = ErrorResponse.Create(e.Message);
            }

            if (errorResponse != null)
                return CreateErrorResponseException(errorResponse);
            else
                return new ErrorResponseException(new Dictionary<string, List<string>>()
                {
                    { "Unknown Error", new List<string>()}
                });
        }
    }
}
