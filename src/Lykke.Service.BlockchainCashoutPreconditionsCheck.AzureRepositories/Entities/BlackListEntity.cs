using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Entities
{
    internal class BlackListEntity : AzureTableEntity
    {
        #region Fields

        // ReSharper disable MemberCanBePrivate.Global

        public string BlockchainIntegrationLayerId { get; set; }
        public string BlockedAddressLowCase { get; set; }
        public string BlockedAddress { get; set; }
        public bool IsCaseSensitiv { get; set; }

        // ReSharper restore MemberCanBePrivate.Global

        #endregion


        #region Keys

        public static string GetPartitionKey(string blockchainIntegrationLayerId)
        {
            return blockchainIntegrationLayerId;
        }

        public static string GetRowKey(string blockedAddress)
        {
            return blockedAddress.ToLower();
        }

        #endregion


        #region Conversion

        public static BlackListEntity FromDomain(BlackListModel model)
        {
            return new BlackListEntity
            {
                PartitionKey = GetPartitionKey(model.BlockchainType),
                RowKey = GetRowKey(model.BlockedAddress),
                BlockchainIntegrationLayerId = model.BlockchainType,
                BlockedAddress = model.BlockedAddress,
                BlockedAddressLowCase = model.BlockedAddress?.ToLower(),
                IsCaseSensitiv = model.IsCaseSensitive,
            };
        }

        public BlackListModel ToDomain()
        {
            return new BlackListModel()
            {
                BlockchainType = this.BlockchainIntegrationLayerId,
                BlockedAddress = this.BlockedAddress,
                BlockedAddressLowCase = this.BlockedAddress?.ToLower(),
                IsCaseSensitive = this.IsCaseSensitiv,
            };
        }

        #endregion
    }
}
