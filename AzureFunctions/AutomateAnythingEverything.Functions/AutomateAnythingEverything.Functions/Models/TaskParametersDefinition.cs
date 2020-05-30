using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomateAnythingEverything.Functions.Models
{
    public class TaskParametersDefinition
    {
        [JsonProperty("parameterName")]
        public string ParameterName { get; set; }

        [JsonProperty("parameterType")]
        public string ParameterType { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }
}
