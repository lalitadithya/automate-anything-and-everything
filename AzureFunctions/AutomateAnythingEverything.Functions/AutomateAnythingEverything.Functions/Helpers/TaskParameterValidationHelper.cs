using AutomateAnythingEverything.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomateAnythingEverything.Functions.Helpers
{
    public static class TaskParameterValidationHelper
    {
        public static bool ValidateTaskParameters(Task task, TaskDefinition taskDefinition, out string errorMessage)
        {
            if(!AreRequiredParametersPresent(task, taskDefinition))
            {
                errorMessage = "Required parameters are not present";
                return false;
            }

            if(!AreParametersCorrectType(task, taskDefinition))
            {
                errorMessage = "Parameters are not of correct type";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private static bool AreRequiredParametersPresent(Task task, TaskDefinition taskDefinition)
        {
            List<string> requiredParameters = taskDefinition.ScriptParameters.Where(x => x.Required).Select(x => x.ParameterName).ToList();
            return requiredParameters.Intersect(task.Parameters.Select(x => x.Name)).Count() == requiredParameters.Count();
        }

        private static bool AreParametersCorrectType(Task task, TaskDefinition taskDefinition)
        {
            foreach(var parameter in task.Parameters)
            {
                string parameterType = taskDefinition.ScriptParameters.Where(x => x.ParameterName == parameter.Name).Select(x => x.ParameterType).FirstOrDefault();
                if(!string.IsNullOrWhiteSpace(parameterType))
                {
                    switch (parameterType)
                    {
                        case "string":
                            break;
                        case "int":
                            if(!int.TryParse(parameter.Value, out _))
                            {
                                return false;
                            }
                            else
                            {
                                break;
                            }
                    }
                }
            }

            return true;
        }
    }
}
