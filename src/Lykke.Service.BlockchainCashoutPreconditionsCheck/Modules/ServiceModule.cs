using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.AzureRepositories.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Repositories;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.Service.BlockchainWallets.Client;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<BlockchainCashoutPreconditionsCheckSettings> _settings;

        private readonly IReloadingManager<BlockchainWalletsServiceClientSettings> _walletClientSettings;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<BlockchainCashoutPreconditionsCheckSettings> settings, IReloadingManager<BlockchainWalletsServiceClientSettings> walletClientSettings)
        {
            _settings = settings;
            _walletClientSettings = walletClientSettings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            var inMemoryCacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            IDistributedCache cache = new MemoryDistributedCache(inMemoryCacheOptions);

            builder.RegisterInstance(cache)
                .As<IDistributedCache>()
                .SingleInstance();

            #region Repo

            builder.Register(c => BlackListRepository.Create(_settings.ConnectionString(x =>x.Db.DataConnString)
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

            builder.Register(c => CreateBlockchainWalletsClient(c.Resolve<ILogFactory>()))
                .As<IBlockchainWalletsClient>()
                .SingleInstance();

            builder.RegisterType<ValidationService>()
                .As<IValidationService>().SingleInstance();

            builder.Populate(_services);
        }

        private IBlockchainWalletsClient CreateBlockchainWalletsClient(ILogFactory logFactory)
        {
            return new BlockchainWalletsClient
            (
                hostUrl: _walletClientSettings.CurrentValue.ServiceUrl,
                logFactory: logFactory
            );
        }
    }
}
