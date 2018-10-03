using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    public class AssetsModule : Module
    {
        private readonly IReloadingManager<AssetSettings> _settings;

        public AssetsModule(IReloadingManager<AssetSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = Assets.Client.AssetServiceSettings.Create(new System.Uri(_settings.CurrentValue.ServiceUrl),
                _settings.CurrentValue.CacheExpirationPeriod);

            ContainerBuilderExtenions.RegisterAssetsClient(builder,
                settings, 
                true);
        }
    }
}
