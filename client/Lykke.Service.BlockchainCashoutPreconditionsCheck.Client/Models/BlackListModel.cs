using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models
{
    public class BlackListModel
    {
        
        /// <summary>
        /// Initializes a new instance of the AddBlackListModel class.
        /// </summary>
        public BlackListModel(string blockchainType, string blockedAddress, bool isCaseSensitive)
        {
            BlockchainType = blockchainType;
            BlockedAddress = blockedAddress;
            IsCaseSensitive = isCaseSensitive;
        }
        public string BlockchainType { get; set; }

        /// <summary>
        /// </summary>
        public string BlockedAddress { get; set; }

        /// <summary>
        /// </summary>
        public bool IsCaseSensitive { get; set; }
    }
}
