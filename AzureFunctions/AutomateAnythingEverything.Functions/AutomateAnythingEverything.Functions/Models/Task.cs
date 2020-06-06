using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomateAnythingEverything.Functions.Models
{
    public class Task
    {
        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("parameters")]
        public List<TaskParameters> Parameters { get; set; }

        [JsonProperty("callbackUri")]
        public string CallbackUri { get; set; }
    }
}
