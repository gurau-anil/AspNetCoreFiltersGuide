﻿using AspNetCoreFiltersGuide.Attributes;
using AspNetCoreFiltersGuide.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFiltersGuide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationFilterController : ControllerBase
    {
        #region Applying Filter as a TypeFilter

        #region Check Authorization Filter without parameter
        // applying Authorization Filter without parameter
        // AuthorizationFilterWithoutParameter can also be applied using a service Filter, as it does not have any parameters.
        [HttpGet]
        [Route("check-authorizationfilter-without-parameter")]
        [TypeFilter(typeof(AuthorizationFilterWithoutParameter))]
        public async Task<IActionResult> CheckAccess()
        {
            return Ok("Access Granted");
        }


        // applying Async Authorization Filter without parameter
        // AuthorizationAsyncFilterWithoutParameter can also be applied using a service Filter, as it also does not have any parameters.
        [HttpGet]
        [Route("check-async-authorizationfilter-without-parameter")]
        [TypeFilter(typeof(AuthorizationAsyncFilterWithoutParameter))]
        public async Task<IActionResult> CheckAccess2()
        {
            return Ok("Access Granted");
        }
        #endregion

        #region Check Authorization Filter with parameter
        // applying Async Authorization Filter with parameter
        // AuthorizationAsyncFilterWithParameter cannot be applied as a Service Filter, as it has arguments to be passed.
        [HttpGet]
        [Route("check-async-authorizationfilter-with-parameter")]
        [TypeFilter(typeof(AuthorizationAsyncFilterWithParameter), Arguments = new Object[] { "paramValue" })]
        public async Task<IActionResult> CheckAccess3()
        {
            return Ok("Access Granted");
        }
        #endregion

        #region Check Authorization Filter with dependency and parameter
        // applying Async Authorization Filter with dependency and parameter
        // AuthorizationAsyncFilterWithParameterAndDependency cannot be applied as a Service Filter, as it has arguments to be passed, but if it has dependencies to be resolved from DI and have no argument/s then we can use ServiceFilter
        [HttpGet]
        [Route("check-async-authorizationfilter-with-parameter-and-dependency")]
        [TypeFilter(typeof(AuthorizationAsyncFilterWithParameterAndDependency), Arguments = new Object[] { "paramValue" })]
        public async Task<IActionResult> CheckAccess4()
        {
            return Ok("Access Granted");
        }
        #endregion

        #endregion

        #region Applying Filter as a ServiceFilter
        //As AuthorizationAsyncFilter does not have any manually passed arguments, which is why it can be used as a Service Filter, however AuthorizationAsyncFilter has to be DI registered
        [HttpGet]
        [Route("check-access-with-servicefilter")]
        [ServiceFilter(typeof(AuthorizationAsyncFilter))]
        public async Task<IActionResult> CheckAccess5()
        {
            return Ok("Access Granted");
        }
        #endregion

        #region Applying Filter combined with Attribute
        //Since CustomAuthorization inherits from TypeFilterAttribute, you do not need to Register the Filter in DI container.
        [HttpGet]
        [Route("check-access-with-attribute")]
        [CustomAuthorization("param")]
        public async Task<IActionResult> CheckAccess6()
        {
            return Ok("Access Granted");
        }
        #endregion
    }
}
