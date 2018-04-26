using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Entities;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Repositories
{
    [UsedImplicitly]
    public class BlackListRepository : IBlackListRepository
    {
        private readonly INoSQLTableStorage<BlackListEntity> _storage;

        public static IBlackListRepository Create(IReloadingManager<string> connectionString, ILog log)
        {
            var storage = AzureTableStorage<BlackListEntity>.Create(
                connectionString,
                "BlackList",
                log);

            return new BlackListRepository(storage);
        }

        private BlackListRepository(INoSQLTableStorage<BlackListEntity> storage)
        {
            _storage = storage;
        }

        public async Task<BlackListModel> GetOrAddAsync(string blockchainType, string blockedAddress, Func<BlackListModel> newAggregateFactory)
        {
            var partitionKey = BlackListEntity.GetPartitionKey(blockchainType);
            var rowKey = BlackListEntity.GetRowKey(blockedAddress);

            var startedEntity = await _storage.GetOrInsertAsync(
                partitionKey,
                rowKey,
                () =>
                {
                    var newAggregate = newAggregateFactory();

                    return BlackListEntity.FromDomain(newAggregate);
                });

            return startedEntity.ToDomain();
        }

        public async Task<BlackListModel> TryGetAsync(string blockchainType, string blockedAddress)
        {
            var partitionKey = BlackListEntity.GetPartitionKey(blockchainType);
            var rowKey = BlackListEntity.GetRowKey(blockedAddress);

            var entity = await _storage.GetDataAsync(partitionKey, rowKey);

            return entity?.ToDomain();
        }

        public async Task<(IEnumerable<BlackListModel>, string continuationToken)> TryGetAllAsync(string blockchainType, int take, string continuationToken = null)
        {
            var partitionKey = BlackListEntity.GetPartitionKey(blockchainType);

            var (entities, newToken) = await _storage.GetDataWithContinuationTokenAsync(partitionKey, take, continuationToken);

            return (entities?.Select(x => x.ToDomain()), newToken);
        }

        public async Task SaveAsync(BlackListModel aggregate)
        {
            var entity = BlackListEntity.FromDomain(aggregate);

            await _storage.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteAsync(string blockchainType, string blockedAddress)
        {
            var partitionKey = BlackListEntity.GetPartitionKey(blockchainType);
            var rowKey = BlackListEntity.GetRowKey(blockedAddress);

            await _storage.DeleteIfExistAsync(partitionKey, rowKey);
        }
    }
}
