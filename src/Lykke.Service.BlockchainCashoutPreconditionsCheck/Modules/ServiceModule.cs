using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.Assets.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.Service.BlockchainWallets.Client;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    [UsedImplicitly]
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var inMemoryCacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(inMemoryCacheOptions);

            builder.RegisterInstance(cache)
                .As<IDistributedCache>()
                .SingleInstance();

            #region Repo

            builder.Register(c => BlackListRepository.Create(_settings.ConnectionString(x =>x.BlockchainCashoutPreconditionsCheckService.Db.DataConnString)
                    , c.Resolve<ILogFactory>()))
                .As<IBlackListRepository>()
                .SingleInstance();

            #endregion 

            builder.RegisterType<AddressExtensionService>()
                .As<AddressExtensionService>()
                .WithParameter("cacheTime", TimeSpan.FromHours(12))
                .SingleInstance();

            builder.RegisterType<BlackListService>()
                .As<IBlackListService>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.Register(c => 
                new BlockchainWalletsClient(_settings.CurrentValue.BlockchainWalletsServiceClient.ServiceUrl, c.Resolve<ILogFactory>()))
            .As<IBlockchainWalletsClient>()
            .SingleInstance();

            builder.RegisterType<ValidationService>()
                .As<IValidationService>().SingleInstance();

            builder.RegisterAssetsClient(
                AssetServiceSettings.Create(new Uri(_settings.CurrentValue.Assets.ServiceUrl), 
                    _settings.CurrentValue.Assets.CacheExpirationPeriod)
            );
        }
    }
}
