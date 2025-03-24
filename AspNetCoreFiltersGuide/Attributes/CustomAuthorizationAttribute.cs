using AspNetCoreFiltersGuide.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFiltersGuide.Attributes
{
    // Instead of using TypeFilter, now we can use CustomAuthorizationAttribute directly 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomAuthorizationAttribute : TypeFilterAttribute<AuthorizationAsyncFilterWithParameterAndDependency>
    {
        public CustomAuthorizationAttribute(string parameters)
        {
            Arguments = new object[] { parameters };
        }
    }

    //If strongly typed TypeFilter is not used then we can do as follow

    //public class CustomAuthorizationAttribute : TypeFilterAttribute
    //{
    //    public CustomAuthorizationAttribute(string someParameters) : base(typeof(AuthorizationAsyncFilterWithParameterAndDependency))
    //    {
    //        Arguments = new object[] { someParameters };
    //    }
    //}
}
