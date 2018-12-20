using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Client;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Services;
using Lykke.SettingsReader;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules
{
    [UsedImplicitly]
    public class BlockchainsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public BlockchainsModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(_settings.CurrentValue.BlockchainsIntegration)
                .AsSelf()
                .SingleInstance();

            //foreach (var blockchain in _settings.CurrentValue.BlockchainsIntegration.Blockchains)
            //{
            //    builder.Register(c =>
            //        {
            //            var log = c.Resolve<ILogFactory>().CreateLog(this);
            //            log.Info("Blockchains registration", 
            //                $"Registering blockchain: {blockchain.Type} -> \r\nAPI: {blockchain.ApiUrl}\r\n");

            //            return blockchain;
            //        })
            //        .Named<BlockchainSettings>(blockchain.Type)
            //        .SingleInstance();

            //    //builder.RegisterType<BlockchainApiClient>()
            //    //    .Named<IBlockchainApiClient>(blockchain.Type)
            //    //    .WithParameter(TypedParameter.From(blockchain.ApiUrl))
            //    //    .SingleInstance();
            //}

            builder.RegisterType<BlockchainApiClientProvider>()
                .As<IBlockchainApiClientProvider>()
                .SingleInstance();

            builder.RegisterType<BlockchainSettingsProvider>()
                .As<IBlockchainSettingsProvider>()
                .SingleInstance();

            builder.RegisterType<BlockchainWalletsCacheService>()
                .As<IBlockchainWalletsCacheService>()
                .As<IInitBlockchainWalletsCacheService>()
                .SingleInstance();
        }
    }
}
