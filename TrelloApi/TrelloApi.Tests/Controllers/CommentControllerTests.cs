using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class CommentControllerTests
{
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly Mock<ILogger<CommentController>> _mockLogger;
    private readonly CommentController _controller;

    public CommentControllerTests()
    {
        _mockCommentService = new Mock<ICommentService>();
        _mockLogger = new Mock<ILogger<CommentController>>();

        _controller = new CommentController(_mockLogger.Object, _mockCommentService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var commentId = 1;
        var outputCommentDto = new OutputCommentDto { Id = 1,  Text = "Comment 1", Date = new DateTime(1), AuthorUsername = "user1"};

        _mockCommentService.Setup(s => s.GetCommentById(commentId, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.GetCommentById(commentId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputCommentDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputCommentDto.Id, value.Id);
        Assert.Equal(outputCommentDto.Text, value.Text);
        Assert.Equal(outputCommentDto.Date, value.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, value.AuthorUsername);
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var commentId = 1;
        var outputCommentDto = (OutputCommentDto?)null;
        
        _mockCommentService.Setup(s => s.GetCommentById(commentId, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.GetCommentById(commentId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task GetCommentsByTaskId_ReturnsOk_WithFullComment()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputCommentDto = new List<OutputCommentDto>
        {
            new OutputCommentDto { Id = 1,  Text = "Comment 1", Date = new DateTime(), AuthorUsername = "user1"},
            new OutputCommentDto { Id = 2,  Text = "Comment 2", Date = new DateTime(), AuthorUsername = "user1"}
        };
        
        _mockCommentService.Setup(s => s.GetCommentsByTaskId(taskId, userId)).ReturnsAsync(listOutputCommentDto);

        var result = await _controller.GetCommentsByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputCommentDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetCommentsByTaskId_ReturnsOk_WithEmptyComment()
    {
        var userId = 1;
        var taskId = 1;
        var listOutputCommentDto = new List<OutputCommentDto>();

        _mockCommentService.Setup(s => s.GetCommentsByTaskId(taskId, userId)).ReturnsAsync(listOutputCommentDto);

        var result = await _controller.GetCommentsByTaskId(taskId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputCommentDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task AddComment_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var taskId = 1;
        var addCommentDto = new AddCommentDto { Text = "Comment 1", AuthorId = 1};
        var outputCommentDto = new OutputCommentDto { Id = 2,  Text = "Comment 2", Date = new DateTime(), AuthorUsername = "user1" };
        
        _mockCommentService.Setup(s => s.AddComment(taskId, addCommentDto, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.AddComment(taskId, addCommentDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputCommentDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputCommentDto.Id, value.Id);
        Assert.Equal(outputCommentDto.Text, value.Text);
        Assert.Equal(outputCommentDto.Date, value.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, value.AuthorUsername);
    }

    [Fact]
    public async Task AddComment_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var taskId = 1;
        var addCommentDto = new AddCommentDto { Text = "Comment 1", AuthorId = 1};
        var outputCommentDto = (OutputCommentDto?)null;
        
        _mockCommentService.Setup(s => s.AddComment(taskId, addCommentDto, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.AddComment(taskId, addCommentDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateComment_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var commentId = 1;
        var updateCommentDto = new UpdateCommentDto { Text = "Comment 1" };
        var outputCommentDto = new OutputCommentDto { Id = 2,  Text = "Comment 2", Date = new DateTime(), AuthorUsername = "user1" };
        
        _mockCommentService.Setup(s => s.UpdateComment(commentId, updateCommentDto, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.UpdateComment(commentId, updateCommentDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputCommentDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputCommentDto.Id, value.Id);
        Assert.Equal(outputCommentDto.Text, value.Text);
        Assert.Equal(outputCommentDto.Date, value.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, value.AuthorUsername);
    }

    [Fact]
    public async Task UpdateComment_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var commentId = 1;
        var updateCommentDto = new UpdateCommentDto { Text = "Comment 1" };
        var outputCommentDto = (OutputCommentDto?)null;
        
        _mockCommentService.Setup(s => s.UpdateComment(commentId, updateCommentDto, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.UpdateComment(commentId, updateCommentDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteComment_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var commentId = 1;
        var outputCommentDto = new OutputCommentDto { Id = 2,  Text = "Comment 2", Date = new DateTime(), AuthorUsername = "user1" };
        
        _mockCommentService.Setup(s => s.DeleteComment(commentId, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.DeleteComment(commentId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputCommentDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputCommentDto.Id, value.Id);
        Assert.Equal(outputCommentDto.Text, value.Text);
        Assert.Equal(outputCommentDto.Date, value.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, value.AuthorUsername);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var commentId = 1;
        var outputCommentDto = (OutputCommentDto?)null;
        
        _mockCommentService.Setup(s => s.DeleteComment(commentId, userId)).ReturnsAsync(outputCommentDto);

        var result = await _controller.DeleteComment(commentId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}