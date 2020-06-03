using AutomateAnythingEverything.Bot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.Services
{
    public class TaskOrchestratorService
    {
        private string apiEndpoint;
        private string callbackBaseUri;
        private HttpClient httpClient;

        public TaskOrchestratorService(IConfiguration configuration)
        {
            apiEndpoint = configuration["TaskOrchestratorUrl"];
            callbackBaseUri = configuration["CallbackBaseUri"];
            httpClient = new HttpClient();
        }

        public async Task<(bool, string)> StartStopVMTask(string vmName, string rgName, string userId, string taskId)
        {
            TaskModel taskModel = new TaskModel
            {
                TaskName = "StopVM",
                CallbackUri = callbackBaseUri + $"?userId={WebUtility.UrlEncode(userId)}&taskId={WebUtility.UrlEncode(taskId)}",
                TaskParameters = new List<TaskParametersModel>
                {
                    new TaskParametersModel
                    {
                        Name = "VMName",
                        Value = vmName
                    },
                    new TaskParametersModel
                    {
                        Name = "ResourceGroupName",
                        Value = rgName
                    }
                }
            };

            //var response = await httpClient.PostAsync(apiEndpoint, new StringContent(JsonConvert.SerializeObject(taskModel)));
            //return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());

            await Task.Delay(100);
            return (true, "12345");
        }
    }
}
