using AutomateAnythingEverything.Functions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(AutomateAnythingEverything.Functions.Startup))]
namespace AutomateAnythingEverything.Functions
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(typeof(CloudStorageAccount), CloudStorageAccount.Parse(configuration["StorageConnetionString"]));
            builder.Services.AddSingleton(typeof(IConfigurationRoot), configuration);
            builder.Services.AddSingleton(typeof(StorageService));
            builder.Services.AddSingleton(typeof(ContainerInstanceService));
        }
    }
}
