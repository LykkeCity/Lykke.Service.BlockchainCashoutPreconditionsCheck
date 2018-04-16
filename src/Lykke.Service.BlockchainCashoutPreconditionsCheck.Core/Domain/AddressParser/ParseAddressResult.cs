using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.AddressParser
{
    public class ParseAddressResult
    {
        public string BaseAddress { get; set; }

        public string AddressExtension { get; set; }
    }
}
