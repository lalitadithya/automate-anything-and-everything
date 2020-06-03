using Microsoft.Bot.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.DataAccessObjects
{
    public class ConversationReferenceDao
    {
        private ConcurrentDictionary<string, ConversationReference> conversationReferences;

        public ConversationReferenceDao()
        {
            conversationReferences = new ConcurrentDictionary<string, ConversationReference>();
        }

        public void SaveConversationReference(string userId, ConversationReference conversationReference)
        {
            conversationReferences.AddOrUpdate(userId, conversationReference, (key, newValue) => conversationReference);
        }

        public ConversationReference GetConversationReference(string userId)
        {
            return conversationReferences[userId];
        }
    }
}
