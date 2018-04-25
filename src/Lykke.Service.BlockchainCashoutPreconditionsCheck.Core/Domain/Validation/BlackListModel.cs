using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation
{
    public class BlackListModel
    {
        public string BlockchainType { get; set; }
        public string BlockedAddressLowCase { get; set; }
        public string BlockedAddress { get; set; }
        public bool IsCaseSensitive { get; set; }
    }
}
