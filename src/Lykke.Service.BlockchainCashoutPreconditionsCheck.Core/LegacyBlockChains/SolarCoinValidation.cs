using System;
using System.Text.RegularExpressions;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.LegacyBlockChains
{
    public class SolarCoinValidation
    {
        private static readonly Regex Base58Regex = new Regex(@"^[123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz]+$");

        public static bool ValidateAddress(String walletAddress)
        {
            try
            {
                if (IsValid(walletAddress))
                {
                    Base58Encoding.DecodeWithCheckSum(walletAddress);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }


        private static bool IsValid(string address)
        {
            return !string.IsNullOrEmpty(address) && address[0] == '8' && address.Length < 40 && Base58Regex.IsMatch(address);
        }
    }
}
