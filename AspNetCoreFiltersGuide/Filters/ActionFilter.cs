using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreFiltersGuide.Filters
{
    public class ActionFilter
    {
    }

public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"Action '{actionName}' is starting.");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"Action '{actionName}' has completed.");
        }
    }

}
