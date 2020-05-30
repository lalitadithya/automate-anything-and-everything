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
            return taskDefinition.ScriptParameters
                                .ToDictionary(x => x.ParameterName, 
                                                x => task.Parameters.Select(y => y.Value).FirstOrDefault(y => x.ParameterName == x.ParameterName));
        }
    }
}
