using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
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
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public AssetsModule(IReloadingManager<AssetSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            Lykke.Service.Assets.Client.ServiceCollectionExtensions.RegisterAssetsClient(_services,
                Assets.Client.AssetServiceSettings.Create(new System.Uri(_settings.CurrentValue.ServiceUrl), _settings.CurrentValue.CacheExpirationPeriod), _log);

            builder.Populate(_services);
        }
    }
}
