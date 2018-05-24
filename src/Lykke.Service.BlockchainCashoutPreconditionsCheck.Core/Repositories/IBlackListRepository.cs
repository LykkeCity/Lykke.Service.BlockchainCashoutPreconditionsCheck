using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories
{
    public  interface IBlackListRepository
    {
        Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress);

        Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take,
            string continuationToken = null);

        Task SaveAsync(BlackListModel model);

        Task DeleteAsync(string blockchainType, string blockedAddress);
    }
}
