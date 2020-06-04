using AutomateAnythingEverything.Bot.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.DataAccessObjects
{
    public class ConversationReferenceDao
    {
        private readonly Container conversationReferencesContainer;

        public ConversationReferenceDao(CosmosClient cosmosClient, IConfiguration configuration)
        {
            conversationReferencesContainer = cosmosClient.GetContainer(configuration["BotStorageDatabaseId"], configuration["BotStorageContainerId"]);
        }

        public void SaveConversationReference(string userId, string taskId, ConversationReference conversationReference, 
            string successMessage, string failureMessage)
        {
            conversationReferencesContainer.CreateItemAsync(new UserTaskRequest
            {
                Id = taskId,
                ConversationReference = JsonConvert.SerializeObject(conversationReference, Formatting.None),
                MessagePromptSuccess = successMessage,
                UserId = userId,
                MessagePromptFailure = failureMessage
            }, new PartitionKey(userId));
        }

        public async Task<(ConversationReference, string, string)> GetConversationReference(string taskId, string userId)
        {
            var result = await conversationReferencesContainer.ReadItemAsync<UserTaskRequest>(taskId, new PartitionKey(userId));
            return (JsonConvert.DeserializeObject<ConversationReference>(result.Resource.ConversationReference), 
                result.Resource.MessagePromptSuccess, result.Resource.MessagePromptFailure);
        }
    }
}
