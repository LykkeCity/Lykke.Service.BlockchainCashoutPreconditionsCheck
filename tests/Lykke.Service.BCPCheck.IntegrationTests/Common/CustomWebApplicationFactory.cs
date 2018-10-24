﻿using Lykke.Service.BlockchainWallets.CTests.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;
using Lykke.Service.BCPCheck.IntegrationTests.Common;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;

namespace Lykke.Service.BlockchainWallets.CTests.Common
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        private readonly LaunchSettingsFixture _fixture;

        public CustomWebApplicationFactory()
        {
            //Loads ENV settings for test
            //_fixture = new LaunchSettingsFixture();
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:5009")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<StartupTests>()
                .UseApplicationInsights();

            return builder;
        }
    }
}
