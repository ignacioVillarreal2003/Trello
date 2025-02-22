using Microsoft.AspNetCore.Mvc;
using TrelloApi.Application.Extensions;
using TrelloApi.Application.Utils;

namespace TrelloApi.Application.Controllers;

[ApiController]
public abstract class BaseController: ControllerBase
{
    protected int UserId => HttpContext.User.GetUserId() 
                            ?? throw new UnauthorizedAccessException("UserId is required but was not found.");
}