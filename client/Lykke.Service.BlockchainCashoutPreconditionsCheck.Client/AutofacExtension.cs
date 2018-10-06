using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public static class AutofacExtension
    {
        [Obsolete]
        public static void RegisterBlockchainCashoutPreconditionsCheckClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<BlockchainCashoutPreconditionsCheckClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<IBlockchainCashoutPreconditionsCheckClient>()
                .SingleInstance();
        }

        public static void RegisterBlockchainCashoutPreconditionsCheckClient(this ContainerBuilder builder, string serviceUrl)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.Register(s => new BlockchainCashoutPreconditionsCheckClient(serviceUrl))
                .As<IBlockchainCashoutPreconditionsCheckClient>()
                .SingleInstance();
        }

        [Obsolete]
        public static void RegisterBlockchainCashoutPreconditionsCheckClient(this ContainerBuilder builder, BlockchainCashoutPreconditionsCheckServiceClientSettings settings, ILog log)
        {
            builder.RegisterBlockchainCashoutPreconditionsCheckClient(settings?.ServiceUrl, log);
        }
    }
}
