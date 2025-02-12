using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Domain.Interfaces.Repositories;

namespace TrelloApi.Tests.Services;

public class ListServiceTests
{
    private readonly Mock<IListRepository> _mockListRepository;
    private readonly Mock<ILogger<ListService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly ListService _service;

    public ListServiceTests()
    {
        _mockListRepository = new Mock<IListRepository>();
        _mockLogger = new Mock<ILogger<ListService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new ListService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockListRepository.Object);
    }
    
    [Fact]
    public async Task GetListById_ReturnsOutputListDto_WhenListExists()
    {
        int listId = 1, userId = 1;
        var list = new List(title: "List 1", boardId: 1, position: 0) { Id = listId };
        var outputListDto = new OutputListDto { Id = list.Id, Title = list.Title, Position = list.Position };

        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync(list);
        _mockMapper.Setup(m => m.Map<OutputListDto>(list)).Returns(outputListDto);

        var result = await _service.GetListById(listId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputListDto.Id, result.Id);
        Assert.Equal(outputListDto.Title, result.Title);
        Assert.Equal(outputListDto.Position, result.Position);
    }

    [Fact]
    public async Task GetListById_ReturnsNull_WhenListDoesNotExist()
    {
        int listId = 1, userId = 1;
        
        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync((List?)null);

        var result = await _service.GetListById(listId, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetListsByBoardId_ReturnsListOfOutputListDto_WhenListsExist()
    {
        int boardId = 1, userId = 1;
        var lists = new List<List>
        {
            new List(title: "List 1", boardId: boardId, position: 0) { Id = 1 },
            new List(title: "List 2", boardId: boardId, position: 1) { Id = 2 }
        };
        var outputListDtos = new List<OutputListDto>
        {
            new OutputListDto { Id = lists[0].Id, Title = lists[0].Title, Position = lists[0].Position },
            new OutputListDto { Id = lists[1].Id, Title = lists[1].Title, Position = lists[1].Position }
        };

        _mockListRepository.Setup(r => r.GetListsByBoardId(boardId)).ReturnsAsync(lists);
        _mockMapper.Setup(m => m.Map<List<OutputListDto>>(lists)).Returns(outputListDtos);

        var result = await _service.GetListsByBoardId(boardId, userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(outputListDtos[0].Id, result[0].Id);
        Assert.Equal(outputListDtos[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetListsByBoardId_ReturnsEmptyList_WhenNoListsExist()
    {
        int boardId = 1, userId = 1;

        _mockListRepository.Setup(r => r.GetListsByBoardId(boardId)).ReturnsAsync(new List<List>());
        _mockMapper.Setup(m => m.Map<List<OutputListDto>>(It.IsAny<List<List>>())).Returns(new List<OutputListDto>());

        var result = await _service.GetListsByBoardId(boardId, userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddList_ReturnsOutputListDto_WhenListIsAdded()
    {
        int boardId = 1, userId = 1;
        var addListDto = new AddListDto { Title = "New List", Position = 0 };
        var newList = new List(title: addListDto.Title, boardId: boardId, position: 0) { Id = 1 };
        var outputListDto = new OutputListDto { Id = newList.Id, Title = newList.Title, Position = newList.Position };

        _mockListRepository.Setup(r => r.AddList(It.IsAny<List>())).ReturnsAsync(newList);
        _mockMapper.Setup(m => m.Map<OutputListDto>(newList)).Returns(outputListDto);

        var result = await _service.AddList(addListDto, boardId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputListDto.Id, result.Id);
        Assert.Equal(outputListDto.Title, result.Title);
        Assert.Equal(outputListDto.Position, result.Position);
    }

    [Fact]
    public async Task AddList_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int boardId = 1, userId = 1;
        var addListDto = new AddListDto { Title = "New List", Position = 0 };

        _mockListRepository.Setup(r => r.AddList(It.IsAny<List>())).ReturnsAsync((List?)null);

        var result = await _service.AddList(addListDto, boardId, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateList_ReturnsOutputListDto_WhenUpdateIsSuccessful()
    {
        int listId = 1, userId = 1;
        var existingList = new List(title: "Old Title", boardId: 1, position: 0) { Id = listId };
        var updateListDto = new UpdateListDto { Title = "New Title" };
        var updatedList = new List(title: updateListDto.Title, boardId: existingList.BoardId, position: existingList.Position) { Id = existingList.Id };
        var outputListDto = new OutputListDto { Id = updatedList.Id, Title = updatedList.Title, Position = updatedList.Position };

        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync(existingList);
        _mockListRepository.Setup(r => r.UpdateList(existingList)).ReturnsAsync(updatedList);
        _mockMapper.Setup(m => m.Map<OutputListDto>(updatedList)).Returns(outputListDto);

        var result = await _service.UpdateList(listId, updateListDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputListDto.Id, result.Id);
        Assert.Equal(outputListDto.Title, result.Title);
        Assert.Equal(outputListDto.Position, result.Position);
    }

    [Fact]
    public async Task UpdateList_ReturnsNull_WhenListNotFound()
    {
        int listId = 1, userId = 1;
        var updateListDto = new UpdateListDto { Title = "New Title" };

        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync((List?)null);

        var result = await _service.UpdateList(listId, updateListDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateList_ReturnsNull_WhenUpdateFails()
    {
        int listId = 1, userId = 1;
        var existingList = new List(title: "Old Title", boardId: 1, position: 0) { Id = listId };
        var updateListDto = new UpdateListDto { Title = "New Title" };

        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync(existingList);
        _mockListRepository.Setup(r => r.UpdateList(existingList)).ReturnsAsync((List?)null);

        var result = await _service.UpdateList(listId, updateListDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteList_ReturnsOutputListDto_WhenDeletionIsSuccessful()
    {
        int listId = 1, userId = 1;
        var existingList = new List(title: "List", boardId: 1, position: 0) { Id = listId };
        var deletedList = existingList;
        var outputListDto = new OutputListDto { Id = deletedList.Id, Title = deletedList.Title, Position = deletedList.Position };

        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync(existingList);
        _mockListRepository.Setup(r => r.DeleteList(existingList)).ReturnsAsync(deletedList);
        _mockMapper.Setup(m => m.Map<OutputListDto>(deletedList)).Returns(outputListDto);

        var result = await _service.DeleteList(listId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputListDto.Id, result.Id);
        Assert.Equal(outputListDto.Title, result.Title);
        Assert.Equal(outputListDto.Position, result.Position);
    }

    [Fact]
    public async Task DeleteList_ReturnsNull_WhenListNotFound()
    {
        int listId = 1, userId = 1;
        
        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync((List?)null);

        var result = await _service.DeleteList(listId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteList_ReturnsNull_WhenDeletionFails()
    {
        int listId = 1, userId = 1;
        var existingList = new List(title: "List 1", boardId: 1, position: 0) { Id = listId };
        
        _mockListRepository.Setup(r => r.GetListById(listId)).ReturnsAsync(existingList);
        _mockListRepository.Setup(r => r.DeleteList(existingList)).ReturnsAsync((List?)null);

        var result = await _service.DeleteList(listId, userId);

        Assert.Null(result);
    }
}