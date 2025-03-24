using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreFiltersGuide.Filters
{
    #region Authorization Filter with and without parameter
    //Creating Authorization Filter without parameter
    public class AuthorizationFilterWithoutParameter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            RequestHandler.HandleRequest(context);
        }
    }

    //Creating Authorization Filter with parameter
    public class AuthorizationFilterWithParameter : IAuthorizationFilter
    {
        private string _parameter1;
        public AuthorizationFilterWithParameter(string parameter1)
        {
            _parameter1 = parameter1;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            RequestHandler.HandleRequest(context);
        }
    }

    #endregion

    #region Async Authorization Filter with and without parameter
    //Creating Async Authorization Filter without parameter
    public class AuthorizationAsyncFilterWithoutParameter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            RequestHandler.HandleRequest(context);
        }
    }

    //Creating Async Authorization Filter with parameter
    public class AuthorizationAsyncFilterWithParameter : IAsyncAuthorizationFilter
    {
        private string _parameter1;
        public AuthorizationAsyncFilterWithParameter(string parameter1)
        {
            _parameter1 = parameter1;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            RequestHandler.HandleRequest(context);
        }
    }

    //Creating Async Authorization Filter with dependency and parameter
    public class AuthorizationAsyncFilterWithParameterAndDependency : IAsyncAuthorizationFilter
    {
        private string _parameter1;
        private readonly ILogger<AuthorizationAsyncFilterWithParameterAndDependency> _logger;
        public AuthorizationAsyncFilterWithParameterAndDependency(ILogger<AuthorizationAsyncFilterWithParameterAndDependency> logger, string parameter1)
        {
            _logger = logger;
            _parameter1 = parameter1;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _logger.LogInformation("Inside Authorization Filter with dependency and parameter");
            RequestHandler.HandleRequest(context);
        }
    }


    //This filter is created to use it as a service filter, however it can be applied it as a type filter
    // For this Filter to be used as a service Filter, it has to be DI registered.
    // Use this as a service Filter when there are no parameters to be passed manually, however it can have Dependencies, which will be resolved from the DI container.
    public class AuthorizationAsyncFilter : IAsyncAuthorizationFilter
    {
        private readonly ILogger<AuthorizationAsyncFilterWithParameterAndDependency> _logger;
        public AuthorizationAsyncFilter(ILogger<AuthorizationAsyncFilterWithParameterAndDependency> logger)
        {
            _logger = logger;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _logger.LogInformation("Inside Authorization Filter with dependency");
            RequestHandler.HandleRequest(context);
        }
    }
    #endregion

    public static class RequestHandler
    {
        public static void HandleRequest(AuthorizationFilterContext context)
        {
            if (new Random().Next(3) % 2 == 0)
            {
                context.Result = new JsonResult(new
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "You do not have the required role to access this resource."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            else
            {
                return;
            }
        }
    }
}
