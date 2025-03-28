using Microsoft.AspNetCore.Authorization;
using TrelloApi.Application.Extensions;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Utils;

public class BoardAccessHandler: AuthorizationHandler<BoardAccessRequirement, int>
{
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<BoardAccessHandler> _logger;
    
    public BoardAccessHandler(IUserBoardRepository userBoardRepository, ILogger<BoardAccessHandler> logger)
    {
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardAccessRequirement requirement, int boardId)
    {
        int? userId = context.User.GetUserId();
        if (!userId.HasValue)
        {
            context.Fail();
            return;
        }
        
        _logger.LogInformation($"Board {boardId} access requested for user {userId}");
        
        UserBoard hasAccess = await _userBoardRepository.GetUserBoardByIdAsync(userId.Value, boardId);
        if (hasAccess != null)
        {
            _logger.LogError($"eNTRA");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogError($"NO eNTRA");
            context.Fail();
        }
    }
}