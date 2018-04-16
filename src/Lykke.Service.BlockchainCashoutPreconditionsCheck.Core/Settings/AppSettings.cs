using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.SlackNotifications;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings
{
    public class AppSettings
    {
        public BlockchainCashoutPreconditionsCheckSettings BlockchainCashoutPreconditionsCheckService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public BlockchainsIntegrationSettings BlockchainsIntegration { get; set; }
        public AssetSettings Assets { get; set; }
        public BlockchainSignFacadeClientSettings BlockchainSignFacadeClient { get; set; }
    }
}
