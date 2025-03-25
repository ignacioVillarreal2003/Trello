using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Extensions;
using TrelloApi.Application.Utils;

namespace TrelloApi.Application.Controllers;

[ApiController]
public abstract class BaseController: ControllerBase
{
    protected int UserId => HttpContext.User.GetUserId() 
                            ?? throw new UnauthorizedAccessException("UserId is required but was not found.");

    protected int BoardId
    {
        get
        {
            if (HttpContext.Items.TryGetValue("BoardId", out var boardIdObj) &&
                boardIdObj is string boardIdStr &&
                int.TryParse(boardIdStr, out int boardId))
            {
                return boardId;
            }

            throw new UnauthorizedAccessException("BoardId is required but was not found.");
        }
    }}