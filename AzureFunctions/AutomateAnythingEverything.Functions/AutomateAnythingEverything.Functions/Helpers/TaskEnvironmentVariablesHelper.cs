using AutomateAnythingEverything.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomateAnythingEverything.Functions.Helpers
{
    public static class TaskEnvironmentVariablesHelper
    {
        public static Dictionary<string, string> ConstructEnvionmentVariables(Task task, TaskDefinition taskDefinition)
        {
            Dictionary<string, string> environmentVariables;

            environmentVariables = taskDefinition.ScriptParameters
                                                .ToDictionary(x => x.ParameterName,
                                                                x => task.Parameters.FirstOrDefault(y => x.ParameterName == y.Name)?.Value);

            if(!string.IsNullOrWhiteSpace(task.CallbackUri))
            {
                environmentVariables.Add("CallbackURI", task.CallbackUri);
            }

            return environmentVariables;
        }
    }
}
