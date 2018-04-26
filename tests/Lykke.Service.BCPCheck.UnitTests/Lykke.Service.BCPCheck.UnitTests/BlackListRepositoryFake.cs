using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;

namespace Lykke.Service.BCPCheck.Tests
{
    public class BlackListRepositoryFake : IBlackListRepository
    {
        private List<BlackListModel> blackList { get; set; }

        public BlackListRepositoryFake()
        {
            blackList = new List<BlackListModel>();
        }

        public async Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress)
        {
            var model = blackList.FirstOrDefault(x => x.BlockchainType == blockchainType &&
                                          x.BlockedAddress?.ToLower() == blockedAddress?.ToLower());

            return model;
        }

        public async Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take, string continuationToken = null)
        {
            var list = blackList.Where(x => x.BlockchainType == blockchainType).Take(take);

            return (list, null);
        }

        public async Task SaveAsync(BlackListModel model)
        {
            await DeleteAsync(model.BlockchainType, model.BlockedAddress);

            blackList.Add(model);
        }

        public async Task DeleteAsync(string blockchainType, string blockedAddress)
        {
            var exisitng = await TryGetAsync(blockchainType, blockedAddress);

            if (exisitng != null)
            {
                blackList.Remove(exisitng);
            }
        }
    }
}
