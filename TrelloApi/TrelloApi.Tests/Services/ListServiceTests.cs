using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class ListServiceTests
{
    private readonly Mock<IListRepository> _mockListRepository;
    private readonly Mock<ILogger<ListService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ListService _service;

    public ListServiceTests()
    {
        _mockListRepository = new Mock<IListRepository>();
        _mockLogger = new Mock<ILogger<ListService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new ListService(
            _mockMapper.Object, 
            _mockUnitOfWork.Object, 
            _mockLogger.Object, 
            _mockListRepository.Object);
    }
    
    [Fact]
    public async Task GetListById_ShouldReturnsList_WhenListFound()
    {
        const int listId = 1;
        var list = new List(title: "title 1", boardId: 1, position: 0) { Id = listId };
        var response = new ListResponse { Id = list.Id, Title = list.Title, Position = list.Position };

        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync(list);
        _mockMapper.Setup(m => m.Map<ListResponse>(list)).Returns(response);

        var result = await _service.GetListById(listId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetListById_ShouldReturnsNull_WhenListNotFound()
    {
        const int listId = 1;
        
        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync((List?)null);

        var result = await _service.GetListById(listId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetListsByBoardId_ShouldReturnsLists_WhenListsFound()
    {
        const int boardId = 1;
        var lists = new List<List>
        {
            new List(title: "title 1", boardId: boardId, position: 0) { Id = 1 },
            new List(title: "title 2", boardId: boardId, position: 1) { Id = 2 }
        };
        var response = new List<ListResponse>
        {
            new ListResponse { Id = lists[0].Id, Title = lists[0].Title, Position = lists[0].Position },
            new ListResponse { Id = lists[1].Id, Title = lists[1].Title, Position = lists[1].Position }
        };

        _mockListRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<List, bool>>>(), null)).ReturnsAsync(lists);
        _mockMapper.Setup(m => m.Map<List<ListResponse>>(It.IsAny<List<List>>())).Returns(response);

        var result = await _service.GetListsByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnsEmptyList_WhenListsNotFound()
    {
        const int boardId = 1;
        
        _mockListRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<List, bool>>>(), null)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<ListResponse>>(It.IsAny<List<List>>())).Returns([]);

        var result = await _service.GetListsByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddList_ShouldReturnsList_WhenAddedSuccessful()
    {
        const int boardId = 1;
        var dto = new AddListDto { Title = "title", Position = 0 };
        var response = new ListResponse { Id = 1, Title = dto.Title, Position = dto.Position };

        _mockListRepository.Setup(r => r.CreateAsync(It.IsAny<List>()));
        _mockMapper.Setup(m => m.Map<ListResponse>(It.IsAny<List>())).Returns(response);

        var result = await _service.AddList(boardId, dto);

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task UpdateList_ShouldReturnsList_WhenUpdatedSuccessful()
    {
        const int listId = 1;
        var list = new List(title: "title", boardId: 1, position: 0) { Id = listId };
        var dto = new UpdateListDto { Title = "updated title" };
        var response = new ListResponse { Id = list.Id, Title = dto.Title, BoardId = list.BoardId, Position = list.Position };

        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync(list);
        _mockListRepository.Setup(r => r.UpdateAsync(It.IsAny<List>()));
        _mockMapper.Setup(m => m.Map<ListResponse>(It.IsAny<List>())).Returns(response);

        var result = await _service.UpdateList(listId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateList_ShouldReturnsNull_WhenUpdatedUnsuccessful()
    {
        const int listId = 1;
        var dto = new UpdateListDto { Title = "updated title" };

        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync((List?)null);

        var result = await _service.UpdateList(listId, dto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteList_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int listId = 1;
        var list = new List(title: "title", boardId: 1, position: 0) { Id = listId };

        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync(list);
        _mockListRepository.Setup(r => r.DeleteAsync(It.IsAny<List>()));

        var result = await _service.DeleteList(listId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteList_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int listId = 1;
        
        _mockListRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<List, bool>>>())).ReturnsAsync((List?)null);

        var result = await _service.DeleteList(listId);

        Assert.False(result);
    }
}
