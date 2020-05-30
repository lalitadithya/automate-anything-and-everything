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

namespace AutomateAnythingEverything.Functions
{
    public class StartTask
    {
        private readonly StorageService storageService;
        private readonly ContainerInstanceService containerInstanceService;
        public StartTask(StorageService storageService, ContainerInstanceService containerInstanceService)
        {
            this.storageService = storageService;
            this.containerInstanceService = containerInstanceService;
        }

        [FunctionName("StartTask")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Models.Task task = JsonConvert.DeserializeObject<Models.Task>(requestBody);

            string jsonTaskDescription = await storageService.GetFileContents(task.TaskName);
            TaskDefinition taskDefinition = JsonConvert.DeserializeObject<TaskDefinition>(jsonTaskDescription);

            if (!TaskParameterValidationHelper.ValidateTaskParameters(task, taskDefinition, out string errorMessage))
            {
                return new BadRequestObjectResult(errorMessage);
            }

            await containerInstanceService.StartContainerInstace(taskDefinition.DockerImage, new System.Collections.Generic.Dictionary<string, string>(), "");

            string name = task.TaskName;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
