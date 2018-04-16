using System.Text.RegularExpressions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.AddressParser;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Services
{
    public class AddressParser:IAddressParser
    {
        public ParseAddressResult ParseAddress(string address, string pattern)
        {
            var regex = new Regex(pattern);
            var groups = regex.Match(address).Groups;

            return new ParseAddressResult
            {
                AddressExtension = groups["addressExtension"].Value,
                BaseAddress = groups["baseAddress"].Value
            };
        }
    }
}
