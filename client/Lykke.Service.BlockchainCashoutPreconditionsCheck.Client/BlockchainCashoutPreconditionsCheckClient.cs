﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public class BlockchainCashoutPreconditionsCheckClient : IBlockchainCashoutPreconditionsCheckClient, IDisposable
    {
        private IBlockchainCashoutPreconditionsCheckAPI _service;

        [Obsolete]
        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl, ILog log)
        {
            _service = new BlockchainCashoutPreconditionsCheckAPI(new Uri(serviceUrl), new HttpClient());
        }

        public BlockchainCashoutPreconditionsCheckClient(string serviceUrl)
        {
            _service = new BlockchainCashoutPreconditionsCheckAPI(new Uri(serviceUrl), new HttpClient());
        }

        /// <summary>
        /// Checks whether or not cashout to the destination address is allowed
        /// </summary>
        /// <param name="validateCashoutModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<(bool isAllowed, IEnumerable<ValidationErrorResponse>)> ValidateCashoutAsync(CashoutValidateModel validateCashoutModel)
        {
            bool isAllowed = false;
            IEnumerable<ValidationErrorResponse> validationErrors;
            var response = await _service.CheckWithHttpMessagesAsync(validateCashoutModel.AssetId, 
                destinationAddress:validateCashoutModel.DestinationAddress,
                amount:validateCashoutModel.Amount,
                clientId:validateCashoutModel.ClientId
                );
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

        /// <summary>
        /// Add new address to the specific blockchain type black list
        /// </summary>
        /// <param name="blackListModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task AddToBlackListAsync(BlackListModel blackListModel)
        {
            var response = await _service.AddAsync(new AddBlackListModel()
            {
                BlockedAddress = blackListModel.BlockedAddress,
                IsCaseSensitive = blackListModel.IsCaseSensitive,
                BlockchainType = blackListModel.BlockchainType,
            });

            if (response == null)
                return;

            switch (response)
            {
                case ErrorResponse errorResponse:
                    if (!string.IsNullOrEmpty(errorResponse.ErrorMessage))
                        throw CreateErrorResponseException(errorResponse);
                    break;
                default:
                    throw new Exception($"Unknown response: {response.ToJson()}");
            }
        }

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<BlackListModel> GetBlackListAsync(string blockchainType, string address)
        {
            var response = await _service.GetAsync(blockchainType, address);

            if (response == null)
                return null;

            switch (response)
            {
                case BlackListResponse blackListResponse:
                    return Map(blackListResponse);
                case ErrorResponse errorResponse:
                    throw CreateErrorResponseException(errorResponse);
                default:
                    throw new Exception($"Unknown response: {response.ToJson()}");
            }
        }

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        public async Task<BlackListEnumerationModel> GetAllBlackListsAsync(string blockchainType, int take, string continuationToken = null)
        {
            var response = await _service.GetAllAsync(blockchainType, take, continuationToken);

            switch (response)
            {
                case BlackListEnumerationResponse blackListResponse:
                    var model = new BlackListEnumerationModel()
                    {
                        List = blackListResponse.List?.Select(x => Map(x)),
                        ContinuationToken = blackListResponse.ContinuationToken
                    };

                    return model;
                case ErrorResponse errorResponse:
                    throw CreateErrorResponseException(errorResponse);
                default:
                    throw new Exception($"Unknown response: {response.ToJson()}");
            }
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
            var response = await _service.DeleteAsync(blockchainType, address);

            if (response == null)
                return;

            switch (response)
            {
                case ErrorResponse errorResponse:
                    if (string.IsNullOrEmpty(errorResponse.ErrorMessage))
                        throw CreateErrorResponseException(errorResponse);
                    break;
                default:
                    throw new Exception($"Unknown response: {response.ToJson()}");
            }
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            _service.Dispose();
            _service = null;
        }

        private BlackListModel Map(BlackListResponse response)
        {
            return new BlackListModel(response.BlockchainType, response.BlockedAddress, response.IsCaseSensitive);
        }

        private ErrorResponseException CreateErrorResponseException(ErrorResponse errorResponse)
        {
            var exception = new ErrorResponseException(errorResponse.ModelErrors);

            return exception;
        }
    }
}
