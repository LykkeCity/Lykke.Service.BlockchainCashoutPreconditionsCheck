using System;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class BlackListService : IBlackListService
    {
        public Task<BlackListModel> GetOrAddAsync(String blockchainType, String blockedAddress, Func newAggregateFactory)
        {
            throw new NotImplementedException();
        }

        public Task<BlackListModel> TryGetAsync(String blockchainType, String blockedAddress)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable, String continuationToken)> TryGetAllAsync(String blockchainType, Int32 take, String continuationToken = default(String))
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(BlackListModel aggregate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(String blockchainType, String blockedAddress)
        {
            throw new NotImplementedException();
        }
    }
}
