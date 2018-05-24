using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation
{
    public class BlackListModel
    {
        public string BlockchainType { get; protected set; }
        public string BlockedAddressLowCase { get; protected set; }
        public string BlockedAddress { get; protected set; }
        public bool IsCaseSensitive { get; protected set; }

        public BlackListModel(string blockchainType, string blockedAddress, bool isCaseSensitive)
        {
            IsCaseSensitive = isCaseSensitive;
            BlockchainType = blockchainType;
            BlockedAddress = blockedAddress;
            BlockedAddressLowCase = blockedAddress.ToLower();
        }
    }
}
