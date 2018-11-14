using System;
using JetBrains.Annotations;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Sdk;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Filter;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BCPCheck.IntegrationTests.Common
{
    public class StartupTests
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "BlockchainCashoutPreconditionsCheck API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.ConfigureMvcOptions = mvcOptions =>
                {
                    mvcOptions.Filters.Add(typeof(CheckModelStateAttribute), 0);
                };

                options.Logs = logs =>
                {
                    logs.UseEmptyLogging();
                };
                
                options.RegisterAdditionalModules = x =>
                {
                    x.RegisterModule<BlockchainsModule>();
                    x.RegisterModule<ServiceModule>();
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.DefaultErrorHandler = ex =>
                {
                    var error = ErrorResponse.Create(ex is ArgumentValidationException 
                        ? "Validation Error" 
                        : "Technical problem");

                    error.AddModelError("exception", ex);

                    return error;
                };
            });
        }
    }
}
