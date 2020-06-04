using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.Models
{
    public class UserTaskRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("messagePromptSuccess")]
        public string MessagePromptSuccess { get; set; }

        [JsonProperty("messagePromptFailure")]
        public string MessagePromptFailure { get; set; }

        [JsonProperty("conversationReference")]
        public string ConversationReference { get; set; }
    }
}
