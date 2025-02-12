using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrelloApi.Application.Utils;

namespace TrelloApi.Application.Filters;

public class RequireAuthenticationAttribute: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.GetUserId();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RequireAuthenticationAttribute>>();

        if (userId == null)
        {
            logger.LogWarning("Unauthorized access attempt.");
            context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized access." });
        }
    }
}