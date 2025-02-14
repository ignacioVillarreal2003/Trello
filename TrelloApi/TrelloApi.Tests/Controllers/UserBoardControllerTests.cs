using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class UserBoardControllerTests
{
    private readonly Mock<IUserBoardService> _mockUserBoardService;
    private readonly Mock<ILogger<UserBoardController>> _mockLogger;
    private readonly UserBoardController _controller;

    public UserBoardControllerTests()
    {
        _mockUserBoardService = new Mock<IUserBoardService>();
        _mockLogger = new Mock<ILogger<UserBoardController>>();

        _controller = new UserBoardController(_mockLogger.Object, _mockUserBoardService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetUserBoardById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var boardId = 1;
        var outputUserBoardDto = new AddUserBoardDto { UserId = 1, BoardId = 1};

        _mockUserBoardService.Setup(s => s.GetUserBoardById(userId, boardId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.GetUserBoardById(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AddUserBoardDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputUserBoardDto.UserId, value.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, value.BoardId);
    }
    
    [Fact]
    public async Task GetUserBoardById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var boardId = 1;
        var outputUserBoardDto = (AddUserBoardDto?)null;
        
        _mockUserBoardService.Setup(s => s.GetUserBoardById(boardId, userId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.GetUserBoardById(boardId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task AddUserBoard_ReturnsOk_WithElementCreated()
    {
        int userId = 1, userToAddId = 1, boardId = 1;
        var outputUserBoardDto = new AddUserBoardDto { UserId = 1, BoardId = 1};
        
        _mockUserBoardService.Setup(s => s.AddUserBoard(userToAddId, boardId, userId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.AddUserBoard(userToAddId, boardId);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<AddUserBoardDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputUserBoardDto.UserId, value.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, value.BoardId);
    }

    [Fact]
    public async Task AddUserBoard_ReturnsBadRequest_WithElementNotCreated()
    {
        int userId = 1, userToAddId = 1, boardId = 1;
        var outputUserBoardDto = (AddUserBoardDto?)null;
        
        _mockUserBoardService.Setup(s => s.AddUserBoard(userToAddId, boardId, userId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.AddUserBoard(userToAddId, boardId);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteUserBoard_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var userToDeleteId = 1;
        var boardId = 1;
        var outputUserBoardDto = new AddUserBoardDto { UserId = 1, BoardId = 1};
        
        _mockUserBoardService.Setup(s => s.DeleteUserBoard(userToDeleteId, boardId, userId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.DeleteUserBoard(userToDeleteId, boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AddUserBoardDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputUserBoardDto.UserId, value.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, value.BoardId);
    }

    [Fact]
    public async Task DeleteUserBoard_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var userToDeleteId = 1;
        var boardId = 1;
        var outputUserBoardDto = (AddUserBoardDto?)null;
        
        _mockUserBoardService.Setup(s => s.DeleteUserBoard(userToDeleteId, boardId, userId)).ReturnsAsync(outputUserBoardDto);

        var result = await _controller.DeleteUserBoard(userToDeleteId, boardId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}