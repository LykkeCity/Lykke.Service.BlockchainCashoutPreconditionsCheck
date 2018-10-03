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
using Microsoft.Extensions.DependencyInjection;

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
            #region Repo

            builder.Register(c => BlackListRepository.Create(_settings.ConnectionString(x =>x.Db.DataConnString)
                    , c.Resolve<ILogFactory>()))
                .As<IBlackListRepository>()
                .SingleInstance();

            #endregion 

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
