using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Functions.Services
{
    public class ContainerInstanceService
    {
        private readonly IConfigurationRoot configuration;
        private readonly IAzure azure;

        public ContainerInstanceService(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
            azure = AuthenticateAndGetAzureInstance();
        }

        public async Task<string> StartContainerInstace(string imageName, Dictionary<string, string> environmentVariables, string scriptLocation)
        {
            var acg = await CreateAndStartContainerInstance(imageName, environmentVariables, scriptLocation, azure);
            return acg.Name;
        }

        private async Task<IContainerGroup> CreateAndStartContainerInstance(string imageName, Dictionary<string, string> environmentVariables, string scriptLocation, IAzure azure)
        {
            return await azure.ContainerGroups
                                .Define(SdkContext.RandomResourceName(configuration["ContainerGroupNamePrefix"], 20))
                                .WithRegion(Region.USWest)
                                .WithExistingResourceGroup(configuration["ResourceGroupName"])
                                .WithLinux()
                                .WithPublicImageRegistryOnly()
                                .WithoutVolume()
                                .DefineContainerInstance(SdkContext.RandomResourceName(configuration["ContainerInstanceNamePrefix"], 20))
                                    .WithImage(imageName)
                                    .WithoutPorts()
                                    .WithCpuCoreCount(1)
                                    .WithMemorySizeInGB(1)
                                    .WithEnvironmentVariables(environmentVariables)
                                    .WithStartingCommandLine("script-runner.sh", scriptLocation)
                                    .Attach()
                                .WithExistingUserAssignedManagedServiceIdentity(await azure.Identities.GetByIdAsync(configuration["UserAssignedManagedServiceIdentityId"]))
                                .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                                .CreateAsync();
        }

        private static IAzure AuthenticateAndGetAzureInstance()
        {
            var creds = SdkContext.AzureCredentialsFactory
                                        .FromSystemAssignedManagedServiceIdentity(MSIResourceType.AppService, AzureEnvironment.AzureGlobalCloud);
            var azure = Microsoft.Azure.Management.Fluent.Azure
                                .Configure()
                                .Authenticate(creds)
                                .WithDefaultSubscription();
            return azure;
        }
    }
}
