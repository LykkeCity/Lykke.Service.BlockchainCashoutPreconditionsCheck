using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;
using Lykke.Service.BCPCheck.IntegrationTests.Common;
using Lykke.Service.BlockchainCashoutPreconditionsCheck;
using Microsoft.Extensions.Configuration;

namespace Lykke.Service.BlockchainWallets.CTests.Common
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
#if DEBUG
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.tests.json", true)
                .Build();
            
            if (!string.IsNullOrEmpty(config["SettingsUrl"]))
                Environment.SetEnvironmentVariable("SettingsUrl", config["SettingsUrl"]);
#endif
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
