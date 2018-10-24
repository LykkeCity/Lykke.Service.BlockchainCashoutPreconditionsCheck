using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string DataConnString { get; set; }
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
