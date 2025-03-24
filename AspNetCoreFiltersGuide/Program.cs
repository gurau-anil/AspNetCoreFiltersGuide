using AspNetCoreFiltersGuide.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Registering Filter, which is to be used as a service Filter. (Note: All the dependencies has to be registered as well)
builder.Services.AddScoped<AuthorizationAsyncFilter>();
builder.Services.AddControllers(options =>
{
    //Registering Global Filter, which will be applied to all the controllers
    //Dependencies will be resolved automatically, however all the dependencies has to be registered
    options.Filters.Add<LoggingActionFilter>(); // also can specify filter execution order

    // If parameters are passed manually then we have to create an instance

    //using var serviceProvider = builder.Services.BuildServiceProvider();
    //var logger = serviceProvider.GetRequiredService<ILogger<LoggingActionFilter>>();
    //options.Filters.Add(new LoggingActionFilter(logger));

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
