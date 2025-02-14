using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Entities.Card;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class CardControllerTests
{
    private readonly Mock<ICardService> _mockTaskService;
    private readonly Mock<ILogger<CardController>> _mockLogger;
    private readonly CardController _controller;

    public CardControllerTests()
    {
        _mockTaskService = new Mock<ICardService>();
        _mockLogger = new Mock<ILogger<CardController>>();

        _controller = new CardController(_mockLogger.Object, _mockTaskService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetTaskById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var taskId = 1;
        var outputTaskDto = new OutputTaskDto { Id = 1,  Title = "Task 1", Description = "", Priority = "Medium", IsCompleted = false};

        _mockTaskService.Setup(s => s.GetTaskById(taskId, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.GetTaskById(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputTaskDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputTaskDto.Id, value.Id);
        Assert.Equal(outputTaskDto.Title, value.Title);
        Assert.Equal(outputTaskDto.Description, value.Description);
        Assert.Equal(outputTaskDto.Priority, value.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, value.IsCompleted);
    }
    
    [Fact]
    public async Task GetTaskById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var taskId = 1;
        var outputTaskDto = (OutputTaskDto?)null;
        
        _mockTaskService.Setup(s => s.GetTaskById(taskId, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.GetTaskById(taskId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task GetTasksByListId_ReturnsOk_WithFullTask()
    {
        var userId = 1;
        var listId = 1;
        var listOutputTaskDto = new List<OutputTaskDto>
        {
            new OutputTaskDto { Id = 1,  Title = "Task 1", Description = "", Priority = "Medium", IsCompleted = false},
            new OutputTaskDto { Id = 2,  Title = "Task 2", Description = "", Priority = "Medium", IsCompleted = false}        
        };
        
        _mockTaskService.Setup(s => s.GetTasksByListId(listId, userId)).ReturnsAsync(listOutputTaskDto);

        var result = await _controller.GetTasksByListId(listId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputTaskDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetTasksByListId_ReturnsOk_WithEmptyTask()
    {
        var userId = 1;
        var listId = 1;
        var listOutputTaskDto = new List<OutputTaskDto>();

        _mockTaskService.Setup(s => s.GetTasksByListId(listId, userId)).ReturnsAsync(listOutputTaskDto);

        var result = await _controller.GetTasksByListId(listId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputTaskDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task AddTask_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var listId = 1;
        var addTaskDto = new AddTaskDto { Title = "Task 1", Description = "" };
        var outputTaskDto = new OutputTaskDto { Id = 1,  Title = "Task 1", Description = "", Priority = "Medium", IsCompleted = false};
        
        _mockTaskService.Setup(s => s.AddTask(listId, addTaskDto, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.AddTask(listId, addTaskDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputTaskDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputTaskDto.Id, value.Id);
        Assert.Equal(outputTaskDto.Title, value.Title);
        Assert.Equal(outputTaskDto.Description, value.Description);
        Assert.Equal(outputTaskDto.Priority, value.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, value.IsCompleted);
    }

    [Fact]
    public async Task AddTask_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var listId = 1;
        var addTaskDto = new AddTaskDto { Title = "Task 1", Description = "" };
        var outputTaskDto = (OutputTaskDto?)null;
        
        _mockTaskService.Setup(s => s.AddTask(listId, addTaskDto, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.AddTask(listId, addTaskDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateTask_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var taskId = 1;
        var updateTaskDto = new UpdateTaskDto { Title = "Task 1" };
        var outputTaskDto = new OutputTaskDto { Id = 1,  Title = "Task 1", Description = "", Priority = "Medium", IsCompleted = false};
        
        _mockTaskService.Setup(s => s.UpdateTask(taskId, updateTaskDto, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.UpdateTask(taskId, updateTaskDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputTaskDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputTaskDto.Id, value.Id);
        Assert.Equal(outputTaskDto.Title, value.Title);
        Assert.Equal(outputTaskDto.Description, value.Description);
        Assert.Equal(outputTaskDto.Priority, value.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, value.IsCompleted);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var taskId = 1;
        var updateTaskDto = new UpdateTaskDto { Title = "Task 1" };
        var outputTaskDto = (OutputTaskDto?)null;
        
        _mockTaskService.Setup(s => s.UpdateTask(taskId, updateTaskDto, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.UpdateTask(taskId, updateTaskDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteTask_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var taskId = 1;
        var outputTaskDto = new OutputTaskDto { Id = 1,  Title = "Task 1", Description = "", Priority = "Medium", IsCompleted = false};
        
        _mockTaskService.Setup(s => s.DeleteTask(taskId, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.DeleteTask(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputTaskDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputTaskDto.Id, value.Id);
        Assert.Equal(outputTaskDto.Title, value.Title);
        Assert.Equal(outputTaskDto.Description, value.Description);
        Assert.Equal(outputTaskDto.Priority, value.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, value.IsCompleted);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var taskId = 1;
        var outputTaskDto = (OutputTaskDto?)null;
        
        _mockTaskService.Setup(s => s.DeleteTask(taskId, userId)).ReturnsAsync(outputTaskDto);

        var result = await _controller.DeleteTask(taskId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}