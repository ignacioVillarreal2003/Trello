using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class CardLabelControllerTests
{
    private readonly Mock<ICardLabelService> _mockTaskLabelService;
    private readonly Mock<ILogger<CardLabelController>> _mockLogger;
    private readonly CardLabelController _controller;

    public CardLabelControllerTests()
    {
        _mockTaskLabelService = new Mock<ICardLabelService>();
        _mockLogger = new Mock<ILogger<CardLabelController>>();

        _controller = new CardLabelController(_mockLogger.Object, _mockTaskLabelService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetTaskLabelById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var taskId = 1;
        var labelId = 1;
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = 1, LabelId = 1};

        _mockTaskLabelService.Setup(s => s.GetTaskLabelById(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.GetTaskLabelById(taskId, labelId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputTaskLabelDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputTaskLabelDto.TaskId, value.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, value.LabelId);
    }
    
    [Fact]
    public async Task GetTaskLabelById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var taskId = 1;
        var labelId = 1;
        var outputTaskLabelDto = (OutputTaskLabelDto?)null;
        
        _mockTaskLabelService.Setup(s => s.GetTaskLabelById(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.GetTaskLabelById(taskId, labelId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task AddTaskLabel_ReturnsOk_WithElementCreated()
    {
        int userId = 1, taskId = 1, labelId = 1;
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = 1, LabelId = 1};
        
        _mockTaskLabelService.Setup(s => s.AddTaskLabel(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.AddTaskLabel(taskId, labelId);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputTaskLabelDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputTaskLabelDto.TaskId, value.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, value.LabelId);
    }

    [Fact]
    public async Task AddTaskLabel_ReturnsBadRequest_WithElementNotCreated()
    {
        int userId = 1, taskId = 1, labelId = 1;
        var outputTaskLabelDto = (OutputTaskLabelDto?)null;
        
        _mockTaskLabelService.Setup(s => s.AddTaskLabel(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.AddTaskLabel(taskId, labelId);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteTaskLabel_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var taskId = 1;
        var labelId = 1;
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = 1, LabelId = 1};
        
        _mockTaskLabelService.Setup(s => s.DeleteTaskLabel(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.DeleteTaskLabel(taskId, labelId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputTaskLabelDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputTaskLabelDto.TaskId, value.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, value.LabelId);
    }

    [Fact]
    public async Task DeleteTaskLabel_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var taskId = 1;
        var labelId = 1;
        var outputTaskLabelDto = (OutputTaskLabelDto?)null;
        
        _mockTaskLabelService.Setup(s => s.DeleteTaskLabel(taskId, labelId, userId)).ReturnsAsync(outputTaskLabelDto);

        var result = await _controller.DeleteTaskLabel(taskId, labelId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}