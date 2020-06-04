using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomateAnythingEverything.Bot.DataAccessObjects;
using AutomateAnythingEverything.Bot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;

namespace AutomateAnythingEverything.Bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskResultController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConversationReferenceDao conversationReferenceDao;

        public TaskResultController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConversationReferenceDao conversationReferenceDao)
        {
            _adapter = adapter;
            _appId = configuration["MicrosoftAppId"];
            this.conversationReferenceDao = conversationReferenceDao;

            if (string.IsNullOrEmpty(_appId))
            {
                _appId = Guid.NewGuid().ToString(); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery]string userId, [FromQuery]string taskId, [FromBody]TaskResultModel taskResultModel)
        {
            var conversation = await conversationReferenceDao.GetConversationReference(taskId, userId);
            string messageToSend = taskResultModel.TaskStatus ? conversation.Item2 : conversation.Item3;

            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversation.Item1, 
                async (context, token) => await BotCallback(messageToSend, context, token), default);
            return Ok();
        }

        private async Task BotCallback(string message, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(message);
        }
    }
}