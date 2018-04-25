using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation
{
    public class BlackListModel
    {
        public string BlockchainIntegrationLayerId { get; set; }
        public string BlockedAddressLowCase { get; set; }
        public string BlockedAddress { get; set; }
        public bool IsCaseSensitiv { get; set; }
        public string MinAmount { get; set; }
        public string MaxAmount { get; set; }
    }
}
