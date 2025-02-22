using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TrelloApi.Application.Filters;

public class ExceptionFilter: IExceptionFilter
{
    private readonly IHostEnvironment _env;

    public ExceptionFilter(IHostEnvironment env)
    {
        _env = env;
    }
    
    public void OnException(ExceptionContext context)
    {
        var traceId = Guid.NewGuid().ToString();
        var statusCode = HttpStatusCode.InternalServerError;

        if (_env.IsDevelopment())
        {
            context.Result = new ObjectResult(new
            {
                message = context.Exception.Message,
                stackTrace = context.Exception.StackTrace,
                traceId
            })
            {
                StatusCode = (int)statusCode
            };
        }
        else
        {
            context.Result = new ObjectResult(new
            {
                message = "An unexpected error has occurred. Please try again later.",
                traceId
            })
            {
                StatusCode = (int)statusCode
            } ;
        }

        context.ExceptionHandled = true;
    }
}