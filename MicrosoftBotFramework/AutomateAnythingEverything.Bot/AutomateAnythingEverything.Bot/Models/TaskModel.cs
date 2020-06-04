using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.Models
{
    public class TaskModel
    {
        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("callbackUri")]
        public string CallbackUri { get; set; }

        [JsonProperty("parameters")]
        public List<TaskParametersModel> TaskParameters { get; set; }
    }

    public class TaskParametersModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
