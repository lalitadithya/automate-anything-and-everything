using AutomateAnythingEverything.Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateAnythingEverything.Bot.Dialogs
{
    public class StopVmDialog : CancelAndHelpDialog
    {
        private const string VmNameStepMsgText = "What is the name of the VM (virtual machine) you would like to stop?";
        private const string RgNameStepMsgText = "What is the name of the resource group in which the VM (virtual machine) is present?";

        public StopVmDialog()
            : base(nameof(StopVmDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                VmNameStepAsync,
                RgNameStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> VmNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var vmDetails = (VmDetails)stepContext.Options;

            if (vmDetails.VmName == null)
            {
                var promptMessage = MessageFactory.Text(VmNameStepMsgText, VmNameStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(vmDetails.VmName, cancellationToken);
        }

        private async Task<DialogTurnResult> RgNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var vmDetails = (VmDetails)stepContext.Options;

            vmDetails.VmName = (string)stepContext.Result;

            if (vmDetails.RgName == null)
            {
                var promptMessage = MessageFactory.Text(RgNameStepMsgText, RgNameStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(vmDetails.RgName, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var vmDetails = (VmDetails)stepContext.Options;

            vmDetails.RgName = (string)stepContext.Result;

            var messageText = $"Please confirm, you would like to stop: {vmDetails.VmName} residing in : {vmDetails.RgName}. Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var vmDetails = (VmDetails)stepContext.Options;

                // do somthing.

                return await stepContext.EndDialogAsync(vmDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
