# Understanding and Implementing Filters

Filters are components that allow you to intercept and process HTTP requests and responses at various stages of the request/response pipeline.

## Why Use Filters?

- Filters enable you to apply logic that affects multiple actions or controllers without duplicating code.
- Filters can modify the incoming request or the outgoing response, allowing you to customize the behavior of your application.
- Filters provide a centralized location for handling common tasks, making your code more maintainable and organized.

## Types of filter

 - Authorization Filter
 - Resource Filter
 - Action Filter
 - Exception Filter
 - Result Filter

Note: Filters are executed in a specific order, typically: Authorization > Resource > Action > Exception (if an exception occurs) > Result.

### 1. Authorization Filters

-   **Purpose:** Authorization filters are the first line of defense in the request pipeline. They determine whether a user is authorized to access a specific resource (action or controller).
-   **Interface:** **IAuthorizationFilter** or **IAsyncAuthorizationFilter**.
-   **Execution:** They execute before any other filter.

### 2. Resource Filters

-   **Purpose:** Resource filters are executed before and after the rest of the filter pipeline. They can short-circuit the pipeline, preventing further execution.
-   **Interface:** **IResourceFilter** or **IAsyncResourceFilter**.
-   **Execution:** They wrap the execution of action and result filters.

### 3. Action Filters

-   **Purpose:** Action filters execute before and after an action method is invoked. They can modify action arguments or the action result.
-   **Interface:** **IActionFilter** or **IAsyncActionFilter**.
-   **Execution:** They wrap the execution of action methods.

### 4. Exception Filters

-   **Purpose:** Exception filters handle unhandled exceptions that occur during the request/response pipeline.
-   **Interface:** **IExceptionFilter** or **IAsyncExceptionFilter**.
-   **Execution:** They execute when an exception is thrown.

### 5. Result Filters

-   **Purpose:** Result filters execute before and after the action result is executed. They can modify the response.
-   **Interface:** **IResultFilter** or **IAsyncResultFilter**.
-   **Execution:** They wrap the execution of action results.

### Creating Filter
---
You have to implement  following interface to create a filter of your choice.
- **Authorization Filter**: `IAuthorizationFilter` or `IAsyncAuthorizationFilter`
- **Resource Filter**: `IResourceFilter` or `IAsyncResourceFilter`
- **Action Filte**r: `IActionFilter` or `IAsyncActionFilter`
- **Exception Filter**: `IExceptionFilter` or `IAsyncExceptionFilter`
- **Result Filter**: `IResultFilter` or `IAsyncResultFilter`

Note: When implementing async versions of these interfaces, the method names change. For example, OnActionExecutionAsync instead of OnActionExecuting and OnActionExecuted.

Examples:

Creating With parameter
```csharp
public class CustomFilterWithParameters : IActionFilter
{
    private readonly ILogger<CustomFilterWithParameters> _logger;
    public CustomFilterWithParameters( ILogger<CustomFilterWithParameters> logger, string parameter)
    {
         _logger = logger;
    }
	public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

Creating Without parameter
```csharp
public class CustomFilterWithoutParameters : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context) { }
}
```

### Applying Filter
---
You can apply Filter in various ways.

**1. [ServiceFilter]**
- **When to use:** When the filter have dependencies that need to be resolved from the DI container, and you don't have additional arguments to pass manually.
- It allows for scoped lifetime management of the filter.
- Dependency Injection registration is required. `services.AddScoped<CustomFilter>();`
- Apply as: `[ServiceFilter(typeof(CustomFilter))]`
- Allows for scoped lifetime management of the filter.
Example:
```csharp
[ServiceFilter(typeof(CustomFilter))]
public IActionResult MyAction()
{
    return Ok();
}
```

**2. [TypeFilter]**
- **When to use:** When filter requires additional parameters (like strings, integers, or specific configuration values) that need to be manually passed when applying the filter.
- No Dependency Injection registration is required.
- Apply as: `[TypeFilter(typeof(CustomFilter), Arguments = new object[] { param1, param2 })]`
- Creates a new instance of the filter for each request.

Examples:
```csharp
[TypeFilter(typeof(CustomFilterWithoutParameters))]
public IActionResult MyAction()
{
    return Ok();
}
```
```csharp
[TypeFilter(typeof(CustomFilterWithParameters), Arguments = new object[] { "Hello, World!" })]
public IActionResult MyAction()
{
    return Ok();
}
```

**3. Global Filter**
- **When to use:** When you want to apply filter in all actions and controllers.

Example:
```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new GlobalExceptionFilter());
});
```
**4. Combined with Attribute**

Approach1:

```csharp
// Create Custom Filter
public class CustomFilterAttribute : Attribute, IActionFilter
{
    private readonly ILogger<CustomFilterAttribute> _logger;
    public CustomFilterAttribute(ILogger<CustomFilterAttribute> logger, string parameter1){
        _logger = logger;
    }
    public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context) { }
}
```
```csharp
// Use custom filter
[CustomFilter("parameter1 Value")]
public IActionResult CustomAction()
{
    return Ok();
}
```
```csharp
// Register Custom Filter
services.AddScoped<CustomFilterAttribute>();
```
Note: This approach doesn't actually resolve dependencies when used as an attribute. The constructor with ILogger will not be called.


Approach2:

```csharp
// Creating Custom Filter 
public class CustomFilterAsync : IAsyncAuthorizationFilter
 {
     private readonly string _role;
     private readonly IMyService _myService;
     public CustomFilterAsync(IMyService myService,string role)
     {
         _role = role;
         _myService = myService;
     }
     public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
     {
         var test = await _myService.GetAsync();
         throw new NotImplementedException();
     }
 }
```
```csharp
// Creating Attribute that implements TypeFilterAttribute
// CustomFilterAttribute is not DI registered as it is a child class of TypeFilterAttribute.
public class CustomFilterAttribute : TypeFilterAttribute<CustomFilterAsync>
{
    public CustomFilterAttribute(string someParameters)
    {
        Arguments = new object[] { someParameters };
    }
}
```
```csharp
// Using CustomFilterAttribute
[CustomFilter("parameter value")]
public IActionResult CustomAction()
{
    return Ok();
}
```

Approach3:

```csharp
//Directly inherit from ActionFilterAttribute or AuthorizationFilterAttribute or the respective base filter attribute class
public class CustomFilterAttribute : ActionFilterAttribute
{
    private readonly ILogger _logger;

    // This constructor will NOT be called by DI
    public CustomFilterAttribute(ILogger<MyCustomFilterAttribute> logger)
    {
        _logger = logger; // This will always be null
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // _logger will be null here
    }
}
```
```csharp
[CustomFilter] // Dependencies not resolved, for that either use [ServiceFilter] or [TypeFilter]
public IActionResult Index()
{
	return Ok();
};
```
Note: This is the standard way to create filter attributes, but doesn't support dependency injection out of the box.

### Filter Ordering
You can control the order of filter execution by implementing the `IOrderedFilter` interface:
```csharp
public class CustomOrderedFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; set; } = 1; // Lower numbers run first

    public void OnActionExecuting(ActionExecutingContext context) { }
    public void OnActionExecuted(ActionExecutedContext context) { }
}
```
