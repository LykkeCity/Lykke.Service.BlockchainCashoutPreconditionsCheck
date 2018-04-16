using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.Service.BlockchainSignFacade.Client;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<BlockchainCashoutPreconditionsCheckSettings> _settings;

        private readonly IReloadingManager<BlockchainSignFacadeClientSettings> _facadeClientSettings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<BlockchainCashoutPreconditionsCheckSettings> settings, IReloadingManager<BlockchainSignFacadeClientSettings> facadeClientSettings, ILog log)
        {
            _settings = settings;
            _facadeClientSettings = facadeClientSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterInstance(CreateBlockchainSignFacadeClient())
                .As<IBlockchainSignFacadeClient>();

            builder.RegisterType<AddressParser>()
                .As<IAddressParser>();

            builder.RegisterType<ValidationService>()
                .As<IValidationService>().SingleInstance();

            builder.Populate(_services);
        }

        private IBlockchainSignFacadeClient CreateBlockchainSignFacadeClient()
        {
            return new BlockchainSignFacadeClient
            (
                hostUrl: _facadeClientSettings.CurrentValue.ServiceUrl,
                apiKey: _settings.CurrentValue.SignFacadeApiKey,
                log: _log
            );
        }
    }
}
