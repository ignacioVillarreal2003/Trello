using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class BoardServiceTests
{
    private readonly Mock<IBoardRepository> _mockBoardRepository;
    private readonly Mock<ILogger<BoardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly Mock<IUserBoardRepository> _mockUserBoardRepository;
    private readonly BoardService _service;

    public BoardServiceTests()
    {
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockLogger = new Mock<ILogger<BoardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();
        _mockUserBoardRepository = new Mock<IUserBoardRepository>();

        _service = new BoardService(
            _mockMapper.Object,
            _mockBoardAuthorizationService.Object,
            _mockBoardRepository.Object,
            _mockUserBoardRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetBoardById_ReturnsBoardDto_WhenFound()
    {
        var board = new Board("title 1", "background-1.svg");
        var outputBoardDto = new OutputBoardDetailsDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Background = "background-1.svg", 
        };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<OutputBoardDetailsDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.GetBoardById(1, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Background, result.Background);
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
            new Board("Title 1", "background-1.svg"),
            new Board("Title 2", "Icon-2")
        };
        var listOutputBoardDto = new List<OutputBoardDetailsDto>()
        {
            new OutputBoardDetailsDto { Id = 1, Title = "Title 1", Background = "background-1.svg" },
            new OutputBoardDetailsDto { Id = 2, Title = "Title 2", Background = "background-1.svg" },
        };

        _mockBoardRepository.Setup(r => r.GetBoardsByUserId(1)).ReturnsAsync(listBoard);
        _mockMapper.Setup(m => m.Map<List<OutputBoardDetailsDto>>(It.IsAny<List<Board>>())).Returns(listOutputBoardDto);

        var result = await _service.GetBoardsByUserId(1);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBoards_ReturnsEmptyList_WhenNoBoardsExist()
    {
        _mockBoardRepository.Setup(r => r.GetBoardsByUserId(1)).ReturnsAsync(new List<Board>());
        _mockMapper.Setup(m => m.Map<List<OutputBoardDetailsDto>>(It.IsAny<List<Board>>())).Returns(new List<OutputBoardDetailsDto>());

        var result = await _service.GetBoardsByUserId(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBoard_ReturnsBoardDto_WhenBoardIsAdded()
    {
        var board = new Board("Title 1", "background-1.svg", "Blue");
        var outputBoardDto = new OutputBoardDetailsDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Background = "background-1.svg"
        };
        var addBoardDto = new AddBoardDto { Title = "Title 1", Background = "background-1.svg" };

        _mockBoardRepository.Setup(r => r.AddBoard(It.IsAny<Board>()));
        _mockUserBoardRepository.Setup(r => r.AddUserBoard(It.IsAny<UserBoard>()));
        _mockMapper.Setup(m => m.Map<OutputBoardDetailsDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.AddBoard(addBoardDto, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Background, result.Background);
    }

    [Fact]
    public async Task AddBoard_ReturnsNull_WhenBoardIsNotAdded()
    {
        var addBoardDto = new AddBoardDto { Title = "Title 1", Background = "background-1.svg" };

        _mockBoardRepository.Setup(r => r.AddBoard(It.IsAny<Board>()));

        var result = await _service.AddBoard(addBoardDto, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsBoardDto_WhenBoardIsUpdated()
    {
        var board = new Board("Title 1", "background-1.svg");
        var outputBoardDto = new OutputBoardDetailsDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Background = "background-1.svg"
        };
        var updateBoardDto = new UpdateBoardDto { Title = "Title 1" };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.UpdateBoard(It.IsAny<Board>()));
        _mockMapper.Setup(m => m.Map<OutputBoardDetailsDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.UpdateBoard(1, updateBoardDto, 1);

        Assert.NotNull(result);
        Assert.Equal(outputBoardDto.Id, result.Id);
        Assert.Equal(outputBoardDto.Title, result.Title);
        Assert.Equal(outputBoardDto.Background, result.Background);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsNull_WhenUpdateFails()
    {
        var board = new Board("Title 1", "background-1.svg");
        var updateBoardDto = new UpdateBoardDto { Title = "Title 1" };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.UpdateBoard(It.IsAny<Board>()));

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
        var board = new Board("Title 1", "background-1.svg");
        var outputBoardDto = new OutputBoardDetailsDto 
        { 
            Id = 1, 
            Title = "Title 1", 
            Background = "background-1.svg"
        };

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.DeleteBoard(It.IsAny<Board>()));
        _mockMapper.Setup(m => m.Map<OutputBoardDetailsDto>(It.IsAny<Board>())).Returns(outputBoardDto);

        var result = await _service.DeleteBoard(1, 1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNull_WhenDeletionFails()
    {
        var board = new Board("Title 1", "background-1.svg");

        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.DeleteBoard(It.IsAny<Board>()));

        var result = await _service.DeleteBoard(1, 1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsNull_WhenBoardNotFound()
    {
        _mockBoardRepository.Setup(r => r.GetBoardById(1)).ReturnsAsync((Board?)null);

        var result = await _service.DeleteBoard(1, 1);

        Assert.True(result);
    }
}