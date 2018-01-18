using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation
{
    public class CashoutModel
    {
        public string AssetId { get; set; }
        public decimal Volume { get; set; }
        public string DestinationAddress { get; set; }
    }
}
