using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class BoardControllerTests
{
    private readonly Mock<IBoardService> _mockBoardService;
    private readonly Mock<ILogger<BoardController>> _mockLogger;
    private readonly BoardController _controller;

    public BoardControllerTests()
    {
        _mockBoardService = new Mock<IBoardService>();
        _mockLogger = new Mock<ILogger<BoardController>>();

        _controller = new BoardController(_mockLogger.Object, _mockBoardService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetBoardById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var boardId = 1;
        var outputBoardDto = new OutputBoardDto { Id = 1,  Title = "board 1", Theme = "Blue", Icon = "Icon-1.png", Description = "", IsArchived = false };

        _mockBoardService.Setup(s => s.GetBoardById(boardId, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.GetBoardById(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputBoardDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputBoardDto.Id, value.Id);
        Assert.Equal(outputBoardDto.Title, value.Title);
        Assert.Equal(outputBoardDto.Theme, value.Theme);
        Assert.Equal(outputBoardDto.Icon, value.Icon);
        Assert.Equal(outputBoardDto.Description, value.Description);
        Assert.Equal(outputBoardDto.IsArchived, value.IsArchived);
    }
    
    [Fact]
    public async Task GetBoardById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var boardId = 1;
        var outputBoardDto = (OutputBoardDto?)null;
        
        _mockBoardService.Setup(s => s.GetBoardById(boardId, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.GetBoardById(boardId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task GetBoards_ReturnsOk_WithFullList()
    {
        var userId = 1;
        var listOutputBoardDto = new List<OutputBoardDto>
        {
            new OutputBoardDto { Id = 1,  Title = "board 1", Theme = "Blue", Icon = "Icon-1.png", Description = "", IsArchived = false },
            new OutputBoardDto { Id = 2,  Title = "board 2", Theme = "Blue", Icon = "Icon-2.png", Description = "", IsArchived = false }
        };

        _mockBoardService.Setup(s => s.GetBoards(userId)).ReturnsAsync(listOutputBoardDto);

        var result = await _controller.GetBoards();
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputBoardDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetBoards_ReturnsOk_WithEmptyList()
    {
        var userId = 1;
        var listOutputBoardDto = new List<OutputBoardDto>();

        _mockBoardService.Setup(s => s.GetBoards(userId)).ReturnsAsync(listOutputBoardDto);

        var result = await _controller.GetBoards();
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputBoardDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task AddBoard_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var addBoardDto = new AddBoardDto { Title = "board 1", Theme = "Blue", Icon = "Icon-1.png" };
        var outputBoardDto = new OutputBoardDto { Id = 1, Title = "board 1", Theme = "Blue", Icon = "Icon-1.png", Description = "", IsArchived = false };
        
        _mockBoardService.Setup(s => s.AddBoard(addBoardDto, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.AddBoard(addBoardDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputBoardDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputBoardDto.Id, value.Id);
        Assert.Equal(outputBoardDto.Title, value.Title);
        Assert.Equal(outputBoardDto.Theme, value.Theme);
        Assert.Equal(outputBoardDto.Icon, value.Icon);
        Assert.Equal(outputBoardDto.Description, value.Description);
        Assert.Equal(outputBoardDto.IsArchived, value.IsArchived);
    }

    [Fact]
    public async Task AddBoard_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var addBoardDto = new AddBoardDto { Title = "board 1", Theme = "Blue", Icon = "Icon-1.png" };
        var outputBoardDto = (OutputBoardDto?)null;
        
        _mockBoardService.Setup(s => s.AddBoard(addBoardDto, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.AddBoard(addBoardDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBoard_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var boardId = 1;
        var updateBoardDto = new UpdateBoardDto { Title = "board 2" };
        var outputBoardDto = new OutputBoardDto { Id = 1, Title = "board 2", Theme = "Blue", Icon = "Icon-1.png", Description = "", IsArchived = false };
        
        _mockBoardService.Setup(s => s.UpdateBoard(boardId, updateBoardDto, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.UpdateBoard(boardId, updateBoardDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputBoardDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputBoardDto.Id, value.Id);
        Assert.Equal(outputBoardDto.Title, value.Title);
        Assert.Equal(outputBoardDto.Theme, value.Theme);
        Assert.Equal(outputBoardDto.Icon, value.Icon);
        Assert.Equal(outputBoardDto.Description, value.Description);
        Assert.Equal(outputBoardDto.IsArchived, value.IsArchived);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var boardId = 1;
        var updateBoardDto = new UpdateBoardDto { Title = "board 2" };
        var outputBoardDto = (OutputBoardDto?)null;
        
        _mockBoardService.Setup(s => s.UpdateBoard(boardId, updateBoardDto, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.UpdateBoard(boardId, updateBoardDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBoard_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var boardId = 1;
        var outputBoardDto = new OutputBoardDto { Id = 1, Title = "board 2", Theme = "Blue", Icon = "Icon-1.png", Description = "", IsArchived = false };
        
        _mockBoardService.Setup(s => s.DeleteBoard(boardId, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.DeleteBoard(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputBoardDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputBoardDto.Id, value.Id);
        Assert.Equal(outputBoardDto.Title, value.Title);
        Assert.Equal(outputBoardDto.Theme, value.Theme);
        Assert.Equal(outputBoardDto.Icon, value.Icon);
        Assert.Equal(outputBoardDto.Description, value.Description);
        Assert.Equal(outputBoardDto.IsArchived, value.IsArchived);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var boardId = 1;
        var outputBoardDto = (OutputBoardDto?)null;
        
        _mockBoardService.Setup(s => s.DeleteBoard(boardId, userId)).ReturnsAsync(outputBoardDto);

        var result = await _controller.DeleteBoard(boardId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);

    }
}