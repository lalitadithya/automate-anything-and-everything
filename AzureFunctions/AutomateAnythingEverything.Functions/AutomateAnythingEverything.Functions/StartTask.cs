using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using AutomateAnythingEverything.Functions.Services;
using AutomateAnythingEverything.Functions.Models;
using AutomateAnythingEverything.Functions.Helpers;
using System.Web.Http;

namespace AutomateAnythingEverything.Functions
{
    public class StartTask
    {
        private readonly IConfigurationRoot configuration;
        private readonly StorageService storageService;
        private readonly ContainerInstanceService containerInstanceService;

        public StartTask(StorageService storageService, ContainerInstanceService containerInstanceService, IConfigurationRoot configuration)
        {
            this.storageService = storageService;
            this.containerInstanceService = containerInstanceService;
            this.configuration = configuration;
        }

        [FunctionName("StartTask")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("Started processing request");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Models.Task task = JsonConvert.DeserializeObject<Models.Task>(requestBody);

                string jsonTaskDescription = await storageService.GetFileContents(task.TaskName, configuration["TaskDefinitionContainerName"]);
                TaskDefinition taskDefinition = JsonConvert.DeserializeObject<TaskDefinition>(jsonTaskDescription);

                log.LogInformation("Validating task parameters");
                if (!TaskParameterValidationHelper.ValidateTaskParameters(task, taskDefinition, out string errorMessage))
                {
                    log.LogInformation("Validating task parameters failed");
                    return new BadRequestObjectResult(errorMessage);
                }

                log.LogInformation("Getting SAS for script");
                string uriForScript = storageService.GetUriForFile(taskDefinition.ScriptName, configuration["TaskScriptsContainerName"]);

                log.LogInformation("Constructing environment variables");
                var environmentVariables = TaskEnvironmentVariablesHelper.ConstructEnvionmentVariables(task, taskDefinition);

                log.LogInformation("Starting container instance");
                string name = await containerInstanceService.StartContainerInstace(taskDefinition.DockerImage, environmentVariables, uriForScript);

                log.LogInformation("Complete! Started container with name " + name);

                return new OkObjectResult(name);
            }
            catch (Exception e)
            {
                log.LogError(e, "Unable to process request");
                return new InternalServerErrorResult();
            }
        }
    }
}
