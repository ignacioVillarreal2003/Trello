using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class BoardServiceTests
{
    private readonly Mock<IBoardRepository> _mockBoardRepository;
    private readonly Mock<ILogger<BoardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserBoardRepository> _mockUserBoardRepository;
    private readonly BoardService _service;

    public BoardServiceTests()
    {
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockLogger = new Mock<ILogger<BoardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserBoardRepository = new Mock<IUserBoardRepository>();

        _service = new BoardService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockBoardRepository.Object,
            _mockUserBoardRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetBoardById_ShouldReturnsBoard_WhenBoardFound()
    {
        const int boardId = 1;
        var board = new Board("title", "background") { Id = boardId };
        var response = new BoardResponse { Id = board.Id, Title = board.Title, Background = board.Background };

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync(board);
        _mockMapper.Setup(m => m.Map<BoardResponse>(It.IsAny<Board>())).Returns(response);

        var result = await _service.GetBoardById(boardId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBoardById_ShouldReturnsNull_WhenBoardNotFound()
    {
        const int boardId = 1;

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync((Board?)null);

        var result = await _service.GetBoardById(boardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnsBoards_WhenBoardsFound()
    {
        const int userId = 1;
        var boards = new List<Board>()
        {
            new Board("title 1", "background"),
            new Board("title 2", "background")
        };
        var response = new List<BoardResponse>()
        {
            new BoardResponse { Id = 1, Title = boards[0].Title, Background = boards[0].Background },
            new BoardResponse { Id = 2, Title = boards[1].Title, Background = boards[1].Background },
        };

        _mockBoardRepository.Setup(r => r.GetBoardsByUserIdAsync(userId)).ReturnsAsync(boards);
        _mockMapper.Setup(m => m.Map<List<BoardResponse>>(It.IsAny<List<Board>>())).Returns(response);

        var result = await _service.GetBoardsByUserId(userId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnsEmptyList_WhenBoardsNotFound()
    {
        const int userId = 1;
        
        _mockBoardRepository.Setup(r => r.GetBoardsByUserIdAsync(userId)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<BoardResponse>>(It.IsAny<List<Board>>())).Returns([]);

        var result = await _service.GetBoardsByUserId(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBoard_ShouldReturnsBoard_WhenAddedSuccessful()
    {
        const int userId = 1, boardId = 1;
        var dto = new AddBoardDto { Title = "title", Background = "background" };
        var response = new BoardResponse { Id = boardId, Title = dto.Title, Background = dto.Background };

        _mockBoardRepository.Setup(r => r.CreateAsync(It.IsAny<Board>()));
        _mockUserBoardRepository.Setup(r => r.CreateAsync(It.IsAny<UserBoard>()));
        _mockMapper.Setup(m => m.Map<BoardResponse>(It.IsAny<Board>())).Returns(response);

        var result = await _service.AddBoard(dto, userId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateBoard_ShouldReturnsBoard_WhenUpdatedSuccessful()
    {
        const int boardId = 1;
        var board = new Board("title", "background");
        var dto = new UpdateBoardDto { Title = "updated title" };
        var response = new BoardResponse { Id = 1, Title = dto.Title, Background = board.Background };

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.UpdateAsync(It.IsAny<Board>()));
        _mockMapper.Setup(m => m.Map<BoardResponse>(It.IsAny<Board>())).Returns(response);

        var result = await _service.UpdateBoard(boardId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateBoard_ShouldReturnsNull_WhenUpdatedUnsuccessful()
    {
        const int boardId = 1;
        var dto = new UpdateBoardDto { Title = "title" };

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync((Board?)null);

        var result = await _service.UpdateBoard(boardId, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteBoard_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int boardId = 1;
        var board = new Board("title", "background");

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync(board);
        _mockBoardRepository.Setup(r => r.DeleteAsync(It.IsAny<Board>()));
        
        var result = await _service.DeleteBoard(boardId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBoard_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int boardId = 1;

        _mockBoardRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Board, bool>>>())).ReturnsAsync((Board?)null);

        var result = await _service.DeleteBoard(boardId);

        Assert.False(result);
    }
}