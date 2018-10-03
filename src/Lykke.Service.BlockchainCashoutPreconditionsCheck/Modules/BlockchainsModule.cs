using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Common.Log;
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

        public BlockchainsModule(IReloadingManager<BlockchainsIntegrationSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var blockchain in _settings.CurrentValue.Blockchains)
            {
                builder.Register(c =>
                    {
                        var log = c.Resolve<ILogFactory>().CreateLog(this);
                        log.WriteInfo("Blockchains registration", "",
                            $"Registering blockchain: {blockchain.Type} -> \r\nAPI: {blockchain.ApiUrl}\r\n");

                        return blockchain;
                    })
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
        }
    }
}
