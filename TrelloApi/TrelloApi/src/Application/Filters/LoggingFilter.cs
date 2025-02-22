using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TrelloApi.Application.Filters;

public class LoggingFilter: IAsyncActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }


    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[Request] {Method} {Path}", context.HttpContext.Request.Method, context.HttpContext.Request.Path);

        var resultContext = await next();
        
        stopwatch.Stop();
        _logger.LogInformation("[Response] Status: {Status}, Time Taken: {Time}ms", resultContext.HttpContext.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}