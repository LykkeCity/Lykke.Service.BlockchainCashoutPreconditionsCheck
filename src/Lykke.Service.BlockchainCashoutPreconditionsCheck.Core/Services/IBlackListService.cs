using Lykke.Service.BlockchainApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IBlackListService
    {
        Task<BlackListModel> GetOrAddAsync(string blockchainType, string blockedAddress,
            Func<BlackListModel> newAggregateFactory);

        Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress);

        Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take,
            string continuationToken = null);

        Task SaveAsync(BlackListModel aggregate);

        Task DeleteAsync(string blockchainType, string blockedAddress);
    }
}
