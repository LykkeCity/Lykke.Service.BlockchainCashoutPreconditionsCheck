using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public interface IBlockchainCashoutPreconditionsCheckClient
    {
        /// <summary>
        /// Checks whether or not cashout to the destination address is allowed
        /// </summary>
        /// <param name="validateCashoutModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        Task<(bool isAllowed, IEnumerable<ValidationErrorResponse>)> ValidateCashoutAsync(CheckCashoutValidityModel validateCashoutModel);

        /// <summary>
        /// Add new address to the specific blockchain type black list
        /// </summary>
        /// <param name="blackListModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        Task AddToBlackListAsync(AddBlackListModel blackListModel);

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        Task<BlackListResponse> GetBlackListAsync(string blockchainType, string address);

        /// <summary>
        /// Get address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        Task<BlackListEnumerationResponse> GetAllBlackListsAsync(string blockchainType, int take, string continuationToken = null);

        /// <summary>
        /// Delete address from specific blockchain type black list
        /// </summary>
        /// <param name="blockchainType">Blockchain Type from Integration Layer</param>
        /// /// <param name="address">Address to delete from black list</param>
        /// <returns></returns>
        /// <exception cref="Exception">Is thrown on wrong usage of service.</exception>
        Task DeleteBlackListAsync(string blockchainType, string address);
    }
}
