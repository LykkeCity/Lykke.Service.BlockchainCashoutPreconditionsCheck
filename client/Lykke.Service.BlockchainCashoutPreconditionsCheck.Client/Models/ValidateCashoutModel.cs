using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models
{
    public class CashoutValidateModel
    {
        public string AssetId { get; set; }

        public decimal Amount { get; set; }

        public string DestinationAddress { get; set; }

        public string DestinationAddressBase { get; set; }
    }
}
