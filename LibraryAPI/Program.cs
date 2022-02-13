using LibraryAPI.DbContexts;
using LibraryAPI.Servicces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

#region Comments
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
// Configure the HTTP request pipeline. 
#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//to refuse any other formates except json , the return is 406 status code Not Acceptable
// enable xml formatter to accepted from header
builder.Services.AddControllers(e =>
    {
        e.ReturnHttpNotAcceptable = true;
        //e.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); //core 2.2
    }).AddNewtonsoftJson(e => e.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()) //serializer for patch method [parsing]
    .AddXmlDataContractSerializerFormatters().  //xml core +3
    ConfigureApiBehaviorOptions(setupAction => setupAction.InvalidModelStateResponseFactory = context => //custom error details & messages 
    {
        var problemDetailsFactory = context.HttpContext.RequestServices
          .GetRequiredService<ProblemDetailsFactory>();
        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);
        //add additional informantions not added by default
        problemDetails.Detail = "See the errors field for details";
        problemDetails.Instance = context.HttpContext.Request.Path;
        //find out which status code to use 
        var actionExcutingContext = context as ActionExecutingContext;
        //if there are modelstate errors & all arguments were correctly found
        // we are dealing with validation errors 
        if ((context.ModelState.ErrorCount > 0) &&
       (actionExcutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
        {
            problemDetails.Type = "http://www.fb.com/mmnear";
            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            problemDetails.Title = "One or more validation error occurred";
            return new UnprocessableEntityObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
        }
        //if one of the arguments wasn't correctly found or coundn't be parsed 
        //we are dealing with null/unparseable input 
        problemDetails.Status = StatusCodes.Status400BadRequest;
        problemDetails.Title = "One or more error on input occurred";
        return new BadRequestObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
    });

// Register Services
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
//  SQL + logging in consol
builder.Services.AddDbContext<CourseLibraryContexts>(e => e.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())).
UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.AddSwaggerGen();
// auto mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

//runtime
//// migrate the database.  Best practice = in Main, using service scope 
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<CourseLibraryContexts>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(e =>
    {
        e.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected Fault happend. Try again later. ");
        });
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.Run();

#region Old data
//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//       new WeatherForecast
//       (
//           DateTime.Now.AddDays(index),
//           Random.Shared.Next(-20, 55),
//           summaries[Random.Shared.Next(summaries.Length)]
//       ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
#endregion