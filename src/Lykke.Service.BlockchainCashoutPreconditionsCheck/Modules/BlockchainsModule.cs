using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    public class BlockchainsModule : Module
    {
        private readonly IReloadingManager<BlockchainsIntegrationSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public BlockchainsModule(IReloadingManager<BlockchainsIntegrationSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var blockchain in _settings.CurrentValue.Blockchains)
            {
                _log.WriteInfo("Blockchains registration", "",
                    $"Registering blockchain: {blockchain.Type} -> \r\nAPI: {blockchain.ApiUrl}\r\n");

                builder.RegisterInstance(blockchain)
                    .Named<BlockchainSettings>(blockchain.Type)
                    .SingleInstance();

                builder.RegisterType<BlockchainApiClient>()
                    .Named<IBlockchainApiClient>(blockchain.Type)
                    .WithParameter(TypedParameter.From(blockchain.ApiUrl))
                    .SingleInstance();
            }

            builder.RegisterType<BlockchainApiClientProvider>()
                .As<IBlockchainApiClientProvider>()
                .SingleInstance();

            builder.RegisterType<BlockchainSettingsProvider>()
                .As<IBlockchainSettingsProvider>()
                .SingleInstance();

            builder.Populate(_services);
        }
    }
}
