using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AutomateAnythingEverything.Functions.Services;

namespace AutomateAnythingEverything.Functions
{
    public class GetLogs
    {
        private readonly ContainerInstanceService containerInstanceService;

        public GetLogs(ContainerInstanceService containerInstanceService)
        {
            this.containerInstanceService = containerInstanceService;
        }

        [FunctionName("GetLogs")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Started processing request!");

            string taskId = req.Query["taskId"];

            try
            {
                string containerLogs = await containerInstanceService.GetLogs(taskId);
                return new OkObjectResult(containerLogs);
            }
            catch (Exception e)
            {
                log.LogInformation(e, "Unable to fetch logs from container");
            }

            string responseMessage = string.IsNullOrEmpty(taskId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {taskId}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
