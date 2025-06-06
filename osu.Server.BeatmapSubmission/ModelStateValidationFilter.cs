// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace osu.Server.BeatmapSubmission
{
    public class ModelStateValidationFilter : IActionFilter
    {
        private readonly ILogger<ModelStateValidationFilter> logger;

        public ModelStateValidationFilter(ILogger<ModelStateValidationFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var errorList = new List<string>();

            foreach (var (key, value) in context.ModelState)
            {
                if (value.Errors.Count == 0)
                    continue;

                foreach (var error in value.Errors)
                    errorList.Add($"{{ field: \"{key}\", message: \"{error.ErrorMessage}\", exception: \"{error.Exception}\" }}");
            }

            logger.LogError($"""
                             Failed to validate model state. Errors:
                             {string.Join(Environment.NewLine, errorList)}
                             """);

            context.Result = new BadRequestResult();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
