using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomateAnythingEverything.Functions.Models
{
    public class TaskParameters
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
