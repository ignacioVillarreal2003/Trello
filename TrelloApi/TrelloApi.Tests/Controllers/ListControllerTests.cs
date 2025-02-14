using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers;

public class ListControllerTests
{
    private readonly Mock<IListService> _mockListService;
    private readonly Mock<ILogger<ListController>> _mockLogger;
    private readonly ListController _controller;

    public ListControllerTests()
    {
        _mockListService = new Mock<IListService>();
        _mockLogger = new Mock<ILogger<ListController>>();

        _controller = new ListController(_mockLogger.Object, _mockListService.Object);
        SetUserId(1);
    }
    
    private void SetUserId(int userId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = userId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    [Fact]
    public async Task GetListById_ReturnsOk_WithElementFound()
    {
        var userId = 1;
        var listId = 1;
        var outputListDto = new OutputListDto { Id = 1,  Title = "List 1", Position = 1 };

        _mockListService.Setup(s => s.GetListById(listId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.GetListById(listId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputListDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputListDto.Id, value.Id);
        Assert.Equal(outputListDto.Title, value.Title);
        Assert.Equal(outputListDto.Position, value.Position);
    }
    
    [Fact]
    public async Task GetListById_ReturnsNotFound_WithElementNotFound()
    {
        var userId = 1;
        var listId = 1;
        var outputListDto = (OutputListDto?)null;
        
        _mockListService.Setup(s => s.GetListById(listId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.GetListById(listId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task GetListsByBoardId_ReturnsOk_WithFullList()
    {
        var userId = 1;
        var boardId = 1;
        var listOutputListDto = new List<OutputListDto>
        {
            new OutputListDto { Id = 1,  Title = "List 1", Position = 1 },
            new OutputListDto { Id = 2,  Title = "List 2", Position = 2 }
        };

        _mockListService.Setup(s => s.GetListsByBoardId(boardId, userId)).ReturnsAsync(listOutputListDto);

        var result = await _controller.GetListsByBoardId(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputListDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(2, value.Count);
    }
    
    [Fact]
    public async Task GetListsByBoardId_ReturnsOk_WithEmptyList()
    {
        var userId = 1;
        var boardId = 1;
        var listOutputListDto = new List<OutputListDto>();

        _mockListService.Setup(s => s.GetListsByBoardId(boardId, userId)).ReturnsAsync(listOutputListDto);

        var result = await _controller.GetListsByBoardId(boardId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<List<OutputListDto>>(objectResult.Value);
            
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Empty(value);
    }
    
    [Fact]
    public async Task AddList_ReturnsOk_WithElementCreated()
    {
        var userId = 1;
        var boardId = 1;
        var addListDto = new AddListDto { Title = "List 1" };
        var outputListDto = new OutputListDto { Id = 1, Title = "List 1", Position = 1};
        
        _mockListService.Setup(s => s.AddList(addListDto, boardId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.AddList(boardId, addListDto);
        var objectResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<OutputListDto>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(outputListDto.Id, value.Id);
        Assert.Equal(outputListDto.Title, value.Title);
        Assert.Equal(outputListDto.Position, value.Position);
    }

    [Fact]
    public async Task AddList_ReturnsBadRequest_WithElementNotCreated()
    {
        var userId = 1;
        var boardId = 1;
        var addListDto = new AddListDto { Title = "List 1" };
        var outputListDto = (OutputListDto?)null;
        
        _mockListService.Setup(s => s.AddList(addListDto, boardId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.AddList(boardId, addListDto);
        var objectResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.Equal(400, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task UpdateList_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var listId = 1;
        var updateListDto = new UpdateListDto { Title = "List 2" };
        var outputListDto = new OutputListDto { Id = 1, Title = "List 2", Position = 1};
        
        _mockListService.Setup(s => s.UpdateList(listId, updateListDto, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.UpdateList(listId, updateListDto);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputListDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputListDto.Id, value.Id);
        Assert.Equal(outputListDto.Title, value.Title);
        Assert.Equal(outputListDto.Position, value.Position);
    }

    [Fact]
    public async Task UpdateList_ReturnsNotFound_WithElementNotUpdated()
    {
        var userId = 1;
        var listId = 1;
        var updateListDto = new UpdateListDto { Title = "List 2" };
        var outputListDto = (OutputListDto?)null;
        
        _mockListService.Setup(s => s.UpdateList(listId, updateListDto, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.UpdateList(listId, updateListDto);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteList_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var listId = 1;
        var outputListDto = new OutputListDto { Id = 1, Title = "List 2", Position = 1};
        
        _mockListService.Setup(s => s.DeleteList(listId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.DeleteList(listId);
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<OutputListDto>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(outputListDto.Id, value.Id);
        Assert.Equal(outputListDto.Title, value.Title);
        Assert.Equal(outputListDto.Position, value.Position);
    }

    [Fact]
    public async Task DeleteList_ReturnsNotFound_WithElementNotDeleted()
    {
        var userId = 1;
        var listId = 1;
        var outputListDto = (OutputListDto?)null;
        
        _mockListService.Setup(s => s.DeleteList(listId, userId)).ReturnsAsync(outputListDto);

        var result = await _controller.DeleteList(listId);
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        
        Assert.Equal(404, objectResult.StatusCode);
    }
}