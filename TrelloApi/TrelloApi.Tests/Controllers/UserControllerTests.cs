using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.User.DTO;
using TrelloApi.Infrastructure.Authentication;

namespace TrelloApi.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IJwt> _mockJwt;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockJwt = new Mock<IJwt>();
        _mockLogger = new Mock<ILogger<UserController>>();

        _controller = new UserController(_mockLogger.Object, _mockUserService.Object, _mockJwt.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [Fact]
    public async Task GetUsers_ReturnsOk_WithFullList()
    {
        var userId = 1;
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
        };

        _mockUserService.Setup(s => s.GetUsers(userId)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsers();
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk_WithEmptyList()
    {
        var userId = 1;
        var listOutputUserDto = new List<OutputUserDto>();

        _mockUserService.Setup(s => s.GetUsers(userId)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsers();
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ReturnsOk_WhitFullList()
    {
        var userId = 1;
        var username = "";
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
        };

        _mockUserService.Setup(s => s.GetUsersByUsername(userId, username)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsersByUsername(username);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ReturnsOk_WhitEmptyList()
    {
        var userId = 1;
        var username = "";
        var listOutputUserDto = new List<OutputUserDto>();
        
        _mockUserService.Setup(s => s.GetUsersByUsername(userId, username)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsersByUsername(username);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task GetUsersByBoardId_ReturnsOk_WhitFullList()
    {
        var userId = 1;
        var boardId = 1;
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
        };

        _mockUserService.Setup(s => s.GetUsersByBoardId(userId, boardId)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsersByBoardId(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetUsersByBoardId_ReturnsOk_WhitEmptyList()
    {
        var userId = 1;
        var boardId = 1;
        var listOutputUserDto = new List<OutputUserDto>();
            
        _mockUserService.Setup(s => s.GetUsersByBoardId(userId, boardId)).ReturnsAsync(listOutputUserDto);

        var result = await _controller.GetUsersByBoardId(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task GetUsersByTaskId_ReturnsOk_WhitFullList()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputUsers = new List<OutputUserDto>
        {
            new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
        };

        _mockUserService.Setup(s => s.GetUsersByTaskId(userId, taskId)).ReturnsAsync(listOutputUsers);

        var result = await _controller.GetUsersByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetUsersByTaskId_ReturnsOk_WhitEmptyList()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputUsers = new List<OutputUserDto>();
        
        _mockUserService.Setup(s => s.GetUsersByTaskId(userId, taskId)).ReturnsAsync(listOutputUsers);

        var result = await _controller.GetUsersByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputUserDto>>(objectResult.Value);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task RegisterUser_ReturnsCreated_WithElementCreated()
    {
        var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" };
        var token = "fake-jwt-token";

        _mockUserService.Setup(s => s.RegisterUser(registerUserDto)).ReturnsAsync(outputUserDto);
        _mockJwt.Setup(j => j.GenerateToken(outputUserDto.Id)).Returns(token);

        var result = await _controller.RegisterUser(registerUserDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = objectResult.Value;
    
        Assert.NotNull(value);

        var tokenValue = value.GetType().GetProperty("token")?.GetValue(value);
        var user = value.GetType().GetProperty("user")?.GetValue(value) as OutputUserDto;

        Assert.NotNull(user);
        Assert.NotNull(tokenValue);
        Assert.Equal(token, tokenValue);
        Assert.Equal(outputUserDto.Id, user!.Id);
        Assert.Equal(outputUserDto.Email, user.Email);
        Assert.Equal(outputUserDto.Username, user.Username);
        Assert.Equal(outputUserDto.Theme, user.Theme);
    }
    
    [Fact]
    public async Task RegisterUser_ReturnsBadRequest_WithElementNotCreated()
    {
        var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
        var outputUserDto = (OutputUserDto?)null;

        _mockUserService.Setup(s => s.RegisterUser(registerUserDto)).ReturnsAsync(outputUserDto);

        var result = await _controller.RegisterUser(registerUserDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task LoginUser_ReturnsOk_WithElementLogged()
    {
        var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" };
        var token = "fake-jwt-token";

        _mockUserService.Setup(s => s.LoginUser(loginUserDto)).ReturnsAsync(outputUserDto);
        _mockJwt.Setup(j => j.GenerateToken(outputUserDto.Id)).Returns(token);

        var result = await _controller.LoginUser(loginUserDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = objectResult.Value;
            
        Assert.NotNull(value);

        var tokenValue = value.GetType().GetProperty("token")?.GetValue(value);
        var user = value.GetType().GetProperty("user")?.GetValue(value) as OutputUserDto;

        Assert.NotNull(user);
        Assert.NotNull(tokenValue);
        Assert.Equal(token, tokenValue);
        Assert.Equal(outputUserDto.Id, user!.Id);
        Assert.Equal(outputUserDto.Email, user.Email);
        Assert.Equal(outputUserDto.Username, user.Username);
        Assert.Equal(outputUserDto.Theme, user.Theme);
    }

    [Fact]
    public async Task LoginUser_ReturnsBadRequest_WithElementNotLogged()
    {
        var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
        var outputUserDto = (OutputUserDto?)null;

        _mockUserService.Setup(s => s.LoginUser(loginUserDto)).ReturnsAsync(outputUserDto);

        var result = await _controller.LoginUser(loginUserDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var updateUserDto = new UpdateUserDto { Username = "user2" };
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user2", Theme = "Light" };

        _mockUserService.Setup(s => s.UpdateUser(userId, updateUserDto)).ReturnsAsync(outputUserDto);

        var result = await _controller.UpdateUser(updateUserDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputUserDto>(objectResult.Value);
        
        Assert.Equal(outputUserDto.Id, value.Id);
        Assert.Equal(outputUserDto.Email, value.Email);
        Assert.Equal(outputUserDto.Username, value.Username);
        Assert.Equal(outputUserDto.Theme, value.Theme);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var updateUserDto = new UpdateUserDto { Username = "user2" };
        var outputUserDto = (OutputUserDto?)null;

        _mockUserService.Setup(s => s.UpdateUser(userId, updateUserDto)).ReturnsAsync(outputUserDto);

        var result = await _controller.UpdateUser(updateUserDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }    
    
    [Fact]
    public async Task DeleteUser_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user2", Theme = "Light" };

        _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(outputUserDto);

        var result = await _controller.DeleteUser();
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputUserDto>(objectResult.Value);
        
        Assert.Equal(outputUserDto.Id, value.Id);
        Assert.Equal(outputUserDto.Email, value.Email);
        Assert.Equal(outputUserDto.Username, value.Username);
        Assert.Equal(outputUserDto.Theme, value.Theme);
    }
    
    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var outputUserDto = (OutputUserDto?)null;

        _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(outputUserDto);

        var result = await _controller.DeleteUser();
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }    
}