using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.AddressParser;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IAddressParser
    {
        ParseAddressResult ParseAddress(string address, string pattern);
    }
}
