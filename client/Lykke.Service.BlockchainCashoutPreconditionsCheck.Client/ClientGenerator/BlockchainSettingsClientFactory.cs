﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Caching;
using Lykke.HttpClientGenerator.Infrastructure;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.ClientGenerator;

namespace Lykke.Service.BlockchainWallets.Client.ClientGenerator
{
    public class BlockchainCashoutPreconditionsApiFactory : IBlockchainCashoutPreconditionsApiFactory
    {
        public IBlockchainCashoutPreconditionsCheckApi CreateNew(BlockchainCashoutPreconditionsClientSettings settings,
            bool withCaching = true,
            IClientCacheManager clientCacheManager = null,
            params DelegatingHandler[] handlers)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            return CreateNew(settings.ServiceUrl, withCaching, clientCacheManager, handlers);
        }

        public IBlockchainCashoutPreconditionsCheckApi CreateNew(string url, bool withCaching = true,
            IClientCacheManager clientCacheManager = null, params DelegatingHandler[] handlers)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var builder = new HttpClientGeneratorBuilder(url);

            if (withCaching)
            {
                //explicit strategy declaration
                builder.WithCachingStrategy(new AttributeBasedCachingStrategy());
            }
            else
            {
                //By default it is AttributeBasedCachingStrategy, so if no caching turn it off
                builder.WithoutCaching();
            }

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    builder.WithAdditionalDelegatingHandler(handler);
                }
            }

            clientCacheManager = clientCacheManager ?? new ClientCacheManager();
            var httpClientGenerator = builder.Create(clientCacheManager);

            return httpClientGenerator.Generate<IBlockchainCashoutPreconditionsCheckApi>();
        }
    }
}
