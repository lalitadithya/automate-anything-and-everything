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

namespace AutomateAnythingEverything.Functions
{
    public class StartTask
    {
        private readonly StorageService storageService;
        public StartTask(StorageService storageService)
        {
            this.storageService = storageService;
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

            string name = task.TaskName;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
