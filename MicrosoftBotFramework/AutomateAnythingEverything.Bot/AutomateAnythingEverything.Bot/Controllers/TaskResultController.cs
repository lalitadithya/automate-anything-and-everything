using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutomateAnythingEverything.Bot.DataAccessObjects;
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
        public async Task<IActionResult> Post([FromQuery]string userId, [FromQuery]string taskId)
        {
            await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReferenceDao.GetConversationReference(userId), 
                async (context, token) => await BotCallback(taskId, context, token), default);
            return Ok();
        }

        private async Task BotCallback(string taskId, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync($"Hi there! I have an update about your task {taskId}");
        }
    }
}