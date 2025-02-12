using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Entities.Label;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.Label.DTO;

namespace TrelloApi.Tests.Controllers;

public class LabelControllerTests
{
    private readonly Mock<ILabelService> _mockLabelService;
    private readonly Mock<ILogger<LabelController>> _mockLogger;
    private readonly LabelController _controller;

    public LabelControllerTests()
    {
        _mockLabelService = new Mock<ILabelService>();
        _mockLogger = new Mock<ILogger<LabelController>>();

        _controller = new LabelController(_mockLogger.Object, _mockLabelService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetLabelById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var labelId = 1;
        var outputLabelDto = new OutputLabelDto { Id = 1,  Title = "Label 1", Color = "Blue" };

        _mockLabelService.Setup(s => s.GetLabelById(labelId, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.GetLabelById(labelId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputLabelDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputLabelDto.Id, value.Id);
        Assert.Equal(outputLabelDto.Title, value.Title);
        Assert.Equal(outputLabelDto.Color, value.Color);
    }
    
    [Fact]
    public async Task GetLabelById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var labelId = 1;
        var outputLabelDto = (OutputLabelDto?)null;
        
        _mockLabelService.Setup(s => s.GetLabelById(labelId, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.GetLabelById(labelId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task GetLabelsByTaskId_ReturnsOk_WithFullLabel()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputLabelDto = new List<OutputLabelDto>
        {
            new OutputLabelDto { Id = 1,  Title = "Label 1", Color = "Blue" },
            new OutputLabelDto { Id = 2,  Title = "Label 2", Color = "Blue" }
        
        };
        
        _mockLabelService.Setup(s => s.GetLabelsByTaskId(taskId, userId)).ReturnsAsync(listOutputLabelDto);

        var result = await _controller.GetLabelsByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputLabelDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetLabelsByTaskId_ReturnsOk_WithEmptyLabel()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputLabelDto = new List<OutputLabelDto>();

        _mockLabelService.Setup(s => s.GetLabelsByTaskId(taskId, userId)).ReturnsAsync(listOutputLabelDto);

        var result = await _controller.GetLabelsByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputLabelDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task AddLabel_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var boardId = 1;
        var addLabelDto = new AddLabelDto { Title = "Label 1", Color = "Blue" };
        var outputLabelDto = new OutputLabelDto { Id = 1,  Title = "Label 1", Color = "Blue" };
        
        _mockLabelService.Setup(s => s.AddLabel(boardId, addLabelDto, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.AddLabel(boardId, addLabelDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputLabelDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputLabelDto.Id, value.Id);
        Assert.Equal(outputLabelDto.Title, value.Title);
        Assert.Equal(outputLabelDto.Color, value.Color);
    }

    [Fact]
    public async Task AddLabel_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var boardId = 1;
        var addLabelDto = new AddLabelDto { Title = "Label 1", Color = "Blue" };
        var outputLabelDto = (OutputLabelDto?)null;
        
        _mockLabelService.Setup(s => s.AddLabel(boardId, addLabelDto, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.AddLabel(boardId, addLabelDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateLabel_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var labelId = 1;
        var updateLabelDto = new UpdateLabelDto { Title = "Label 1" };
        var outputLabelDto = new OutputLabelDto { Id = 1,  Title = "Label 1", Color = "Blue" };
        
        _mockLabelService.Setup(s => s.UpdateLabel(labelId, updateLabelDto, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.UpdateLabel(labelId, updateLabelDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputLabelDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputLabelDto.Id, value.Id);
        Assert.Equal(outputLabelDto.Title, value.Title);
        Assert.Equal(outputLabelDto.Color, value.Color);
    }

    [Fact]
    public async Task UpdateLabel_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var labelId = 1;
        var updateLabelDto = new UpdateLabelDto { Title = "Label 1" };
        var outputLabelDto = (OutputLabelDto?)null;
        
        _mockLabelService.Setup(s => s.UpdateLabel(labelId, updateLabelDto, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.UpdateLabel(labelId, updateLabelDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteLabel_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var labelId = 1;
        var outputLabelDto = new OutputLabelDto { Id = 1,  Title = "Label 1", Color = "Blue" };
        
        _mockLabelService.Setup(s => s.DeleteLabel(labelId, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.DeleteLabel(labelId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputLabelDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputLabelDto.Id, value.Id);
        Assert.Equal(outputLabelDto.Title, value.Title);
        Assert.Equal(outputLabelDto.Color, value.Color);
    }

    [Fact]
    public async Task DeleteLabel_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var labelId = 1;
        var outputLabelDto = (OutputLabelDto?)null;
        
        _mockLabelService.Setup(s => s.DeleteLabel(labelId, userId)).ReturnsAsync(outputLabelDto);

        var result = await _controller.DeleteLabel(labelId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}