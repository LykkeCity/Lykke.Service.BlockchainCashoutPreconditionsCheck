using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BlockchainCashoutPreconditionsCheckSettings BlockchainCashoutPreconditionsCheckService { get; set; }
        public BlockchainsIntegrationSettings BlockchainsIntegration { get; set; }
        public AssetSettings Assets { get; set; }
        public BlockchainWalletsServiceClientSettings BlockchainWalletsServiceClient { get; set; }
    }
}
