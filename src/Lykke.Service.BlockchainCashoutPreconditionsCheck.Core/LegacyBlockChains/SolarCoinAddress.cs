using System.Text.RegularExpressions;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.LegacyBlockChains
{
    public class SolarCoinAddress
    {
        private static readonly Regex Base58Regex = new Regex(@"^[123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz]+$");

        public static bool IsValid(string address)
        {
            return !string.IsNullOrEmpty(address) && address[0] == '8' && address.Length < 40 && Base58Regex.IsMatch(address);
        }
    }
}
