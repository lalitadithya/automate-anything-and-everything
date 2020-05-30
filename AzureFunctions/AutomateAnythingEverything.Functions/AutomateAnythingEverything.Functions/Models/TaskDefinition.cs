using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomateAnythingEverything.Functions.Models
{
    public class TaskDefinition
    {
        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("dockerImage")]
        public string DockerImage { get; set; }

        [JsonProperty("scriptName")]
        public string ScriptName { get; set; }

        [JsonProperty("scriptParameters")]
        public List<TaskParametersDefinition> ScriptParameters { get; set; }
    }
}
