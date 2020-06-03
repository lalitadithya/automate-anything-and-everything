// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Luis;
using AutomateAnythingEverything.Bot.Models;
using AutomateAnythingEverything.Bot.Services;

namespace AutomateAnythingEverything.Bot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly A3ERecognizer _luisRecognizer;
        private readonly TaskOrchestratorService taskOrchestratorService;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(A3ERecognizer luisRecognizer, StopVmDialog stopVmDialog, ILogger<MainDialog> logger,
            TaskOrchestratorService taskOrchestratorService)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;
            this.taskOrchestratorService = taskOrchestratorService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(stopVmDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like \"Stop VM vm-name in resource group rg-name\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisRecognizer.RecognizeAsync<A3ECognitiveModel>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case A3ECognitiveModel.Intent.Azure_Resource_VM_Stop:
                    var vmDetails = new VmDetails
                    {
                        VmName = luisResult.Entities.VmName?.FirstOrDefault(),
                        RgName = luisResult.Entities.RgName?.FirstOrDefault()
                    };

                    return await stepContext.BeginDialogAsync(nameof(StopVmDialog), vmDetails, cancellationToken);
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
            // the Result here will be null.
            if (stepContext.Result is VmDetails result)
            {
                _ = Task.Run(async () => await taskOrchestratorService.StartStopVMTask(result.VmName, result.RgName));

                var messageText = $"I have started to work on stopping {result.VmName} in {result.RgName}. I will let you know when I'm done";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
