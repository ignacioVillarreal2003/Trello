using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Services;

public class BoardServiceTests
{
    private readonly Mock<IBoardRepository> _mockBoardRepository;
    private readonly Mock<ILogger<BoardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly Mock<IUserBoardService> _mockUserBoardService;
    private readonly BoardService _service;

    public BoardServiceTests()
    {
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockLogger = new Mock<ILogger<BoardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();
        _mockUserBoardService = new Mock<IUserBoardService>();

        _service = new BoardService(
            _mockMapper.Object,
            _mockBoardAuthorizationService.Object,
            _mockUserBoardService.Object,
            _mockBoardRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetBoardById_ReturnsBoardDto_WhenFound()
    {
        var board = new Board("title 1", "icon-1");
        var outputBoardDto = new OutputBoardDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Theme = "Blue", 
            Icon = "Icon-1", 
            Description = "", 
            IsArchived = false 
        };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<OutputBoardDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.GetBoardById(1, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Theme, result.Theme);
        Assert.Equal(outputBoardDto.Icon, result.Icon);
        Assert.Equal(outputBoardDto.Description, result.Description);
        Assert.Equal(outputBoardDto.IsArchived, result.IsArchived);
    }

    [Fact]
    public async Task GetBoardById_ReturnsNull_WhenBoardNotFound()
    {
        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync((Board?)null);

        var result = await _service.GetBoardById(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBoards_ReturnsFullList_WhenBoardsExist()
    {
        var listBoard = new List<Board>()
        {
            new Board("Title 1", "Icon-1"),
            new Board("Title 2", "Icon-2")
        };
        var listOutputBoardDto = new List<OutputBoardDto>()
        {
            new OutputBoardDto { Id = 1, Title = "Title 1", Theme = "Blue", Icon = "Icon-1", Description = "", IsArchived = false },
            new OutputBoardDto { Id = 2, Title = "Title 2", Theme = "Blue", Icon = "Icon-2", Description = "", IsArchived = false },
        };

        _mockBoardRepository.Setup(r => r.GetBoards(1)).ReturnsAsync(listBoard);
        _mockMapper.Setup(m => m.Map<List<OutputBoardDto>>(It.IsAny<List<Board>>())).Returns(listOutputBoardDto);

        var result = await _service.GetBoards(1);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBoards_ReturnsEmptyList_WhenNoBoardsExist()
    {
        _mockBoardRepository.Setup(r => r.GetBoards(1)).ReturnsAsync(new List<Board>());
        _mockMapper.Setup(m => m.Map<List<OutputBoardDto>>(It.IsAny<List<Board>>())).Returns(new List<OutputBoardDto>());

        var result = await _service.GetBoards(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBoard_ReturnsBoardDto_WhenBoardIsAdded()
    {
        var board = new Board("Title 1", "Icon-1", "Blue");
        var outputBoardDto = new OutputBoardDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Theme = "Blue", 
            Icon = "Icon-1", 
            Description = "", 
            IsArchived = false 
        };
        var addBoardDto = new AddBoardDto { Title = "Title 1", Theme = "Blue", Icon = "Icon-1" };

        _mockBoardRepository.Setup(r => r.AddBoard(It.IsAny<Board>())).ReturnsAsync(board);
        _mockUserBoardService.Setup(s => s.AddUserBoard(It.IsAny<AddUserBoardDto>(), It.IsAny<int>()));
        _mockMapper.Setup(m => m.Map<OutputBoardDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.AddBoard(addBoardDto, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Theme, result.Theme);
        Assert.Equal(outputBoardDto.Icon, result.Icon);
        Assert.Equal(outputBoardDto.Description, result.Description);
        Assert.Equal(outputBoardDto.IsArchived, result.IsArchived);
    }

    [Fact]
    public async Task AddBoard_ReturnsNull_WhenBoardIsNotAdded()
    {
        var addBoardDto = new AddBoardDto { Title = "Title 1", Theme = "Blue", Icon = "Icon-1" };

        _mockBoardRepository.Setup(r => r.AddBoard(It.IsAny<Board>())).ReturnsAsync((Board?)null);

        var result = await _service.AddBoard(addBoardDto, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsBoardDto_WhenBoardIsUpdated()
    {
        var board = new Board("Title 1", "Icon-1");
        var outputBoardDto = new OutputBoardDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Theme = "Blue", 
            Icon = "Icon-1", 
            Description = "", 
            IsArchived = false 
        };
        var updateBoardDto = new UpdateBoardDto { Title = "Title 1" };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.UpdateBoard(It.IsAny<Board>())).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<OutputBoardDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.UpdateBoard(1, updateBoardDto, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Theme, result.Theme);
        Assert.Equal(outputBoardDto.Icon, result.Icon);
        Assert.Equal(outputBoardDto.Description, result.Description);
        Assert.Equal(outputBoardDto.IsArchived, result.IsArchived);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsNull_WhenUpdateFails()
    {
        var board = new Board("Title 1", "Icon-1");
        var updateBoardDto = new UpdateBoardDto { Title = "Title 1" };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.UpdateBoard(It.IsAny<Board>())).ReturnsAsync((Board?)null);

        var result = await _service.UpdateBoard(1, updateBoardDto, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsNull_WhenBoardNotFound()
    {
        var updateBoardDto = new UpdateBoardDto { Title = "Title 1" };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync((Board?)null);

        var result = await _service.UpdateBoard(1, updateBoardDto, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsBoardDto_WhenBoardIsDeleted()
    {
        var board = new Board("Title 1", "Icon-1");
        var outputBoardDto = new OutputBoardDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Theme = "Blue", 
            Icon = "Icon-1", 
            Description = "", 
            IsArchived = false 
        };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.DeleteBoard(It.IsAny<Board>())).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<OutputBoardDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.DeleteBoard(1, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Theme, result.Theme);
        Assert.Equal(outputBoardDto.Icon, result.Icon);
        Assert.Equal(outputBoardDto.Description, result.Description);
        Assert.Equal(outputBoardDto.IsArchived, result.IsArchived);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNull_WhenDeletionFails()
    {
        var board = new Board("Title 1", "Icon-1");

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.DeleteBoard(It.IsAny<Board>())).ReturnsAsync((Board?)null);

        var result = await _service.DeleteBoard(1, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNull_WhenBoardNotFound()
    {
        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync((Board?)null);

        var result = await _service.DeleteBoard(1, 1);

        Assert.Null(result);
    }
}