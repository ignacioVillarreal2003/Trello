using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.UserTask.Dto;

namespace TrelloApi.Tests.Controllers;

public class UserTaskControllerTests
{
    private readonly Mock<IUserTaskService> _mockUserTaskService;
    private readonly Mock<ILogger<UserTaskController>> _mockLogger;
    private readonly UserTaskController _controller;

    public UserTaskControllerTests()
    {
        _mockUserTaskService = new Mock<IUserTaskService>();
        _mockLogger = new Mock<ILogger<UserTaskController>>();

        _controller = new UserTaskController(_mockLogger.Object, _mockUserTaskService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetUserTaskById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var taskId = 1;
        var outputUserTaskDto = new OutputUserTaskDto { UserId = 1, TaskId = 1};

        _mockUserTaskService.Setup(s => s.GetUserTaskById(userId, taskId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.GetUserTaskById(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputUserTaskDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputUserTaskDto.UserId, value.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, value.TaskId);
    }
    
    [Fact]
    public async Task GetUserTaskById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var taskId = 1;
        var outputUserTaskDto = (OutputUserTaskDto?)null;
        
        _mockUserTaskService.Setup(s => s.GetUserTaskById(taskId, userId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.GetUserTaskById(taskId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task AddUserTask_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var addUserTaskDto = new AddUserTaskDto { UserId = 1, TaskId = 1 };
        var outputUserTaskDto = new OutputUserTaskDto { UserId = 1, TaskId = 1};
        
        _mockUserTaskService.Setup(s => s.AddUserTask(addUserTaskDto, userId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.AddUserTask(addUserTaskDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputUserTaskDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputUserTaskDto.UserId, value.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, value.TaskId);
    }

    [Fact]
    public async Task AddUserTask_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var addUserTaskDto = new AddUserTaskDto { UserId = 1, TaskId = 1 };
        var outputUserTaskDto = (OutputUserTaskDto?)null;
        
        _mockUserTaskService.Setup(s => s.AddUserTask(addUserTaskDto, userId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.AddUserTask(addUserTaskDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteUserTask_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var userToDeleteId = 1;
        var taskId = 1;
        var outputUserTaskDto = new OutputUserTaskDto { UserId = 1, TaskId = 1};
        
        _mockUserTaskService.Setup(s => s.DeleteUserTask(userToDeleteId, taskId, userId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.DeleteUserTask(userToDeleteId, taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputUserTaskDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputUserTaskDto.UserId, value.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, value.TaskId);
    }

    [Fact]
    public async Task DeleteUserTask_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var userToDeleteId = 1;
        var taskId = 1;
        var outputUserTaskDto = (OutputUserTaskDto?)null;
        
        _mockUserTaskService.Setup(s => s.DeleteUserTask(userToDeleteId, taskId, userId)).ReturnsAsync(outputUserTaskDto);

        var result = await _controller.DeleteUserTask(userToDeleteId, taskId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}