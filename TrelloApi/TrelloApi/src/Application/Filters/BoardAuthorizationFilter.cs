using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrelloApi.Application.Extensions;
using TrelloApi.Application.Services;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Filters;

public class BoardAuthorizationFilter: IAsyncActionFilter
{
    private readonly IBoardAuthorizationService _authorizationService;

    public BoardAuthorizationFilter(IBoardAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.GetUserId();

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Board-Id", out var boardIdStr) ||
            !int.TryParse(boardIdStr, out int boardId) || userId == null)
        {
            context.Result = new BadRequestObjectResult("Board ID is required.");
            return;
        }

        bool hasAccess = await _authorizationService.HasAccessToBoard(userId.Value, boardId);
        if (!hasAccess)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}

/* [ServiceFilter(typeof(BoardAuthorizationFilter))] */