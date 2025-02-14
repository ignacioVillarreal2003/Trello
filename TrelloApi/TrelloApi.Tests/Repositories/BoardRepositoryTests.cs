using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;

namespace TrelloApi.Tests.Repositories;

public class BoardRepositoryTests
{
    private readonly BoardRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<BoardRepository>> _mockLogger;

    public BoardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<BoardRepository>>();
        _repository = new BoardRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetBoardById_ReturnsBoard_WhenBoardExists()
    {
        int boardId = 1;
        var board = new Board(title: "Test Board", icon: "TestIcon", theme: "TestTheme") { Id = boardId };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoardById(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(boardId, result.Id);
    }

    [Fact]
    public async Task GetBoardById_ReturnsNull_WhenBoardDoesNotExist()
    {
        int boardId = 1;
        
        var result = await _repository.GetBoardById(boardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBoards_ReturnsBoards_WhenBoardsExistForUser()
    {
        int userId = 1;
        var board1 = new Board(title: "Board 1", icon: "Icon1", theme: "Theme1") { Id = 1 };
        var board2 = new Board(title: "Board 2", icon: "Icon2", theme: "Theme2") { Id = 2 };
        var userBoard1 = new UserBoard(userId: userId, boardId: 1);
        var userBoard2 = new UserBoard(userId: userId, boardId: 2);

        _context.Boards.AddRange(board1, board2);
        _context.UserBoards.AddRange(userBoard1, userBoard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoards(userId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetBoards_ReturnsEmptyList_WhenNoBoardsExistForUser()
    {
        int userId = 1;
        
        var result = await _repository.GetBoards(userId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBoard_ReturnsBoard_WhenBoardIsAddedSuccessfully()
    {
        var board = new Board(title: "New Board", icon: "NewIcon", theme: "NewTheme") { Id = 1 };
        
        _context.Boards.RemoveRange(_context.Boards);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddBoard(board);
        
        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
    }

    [Fact]
    public async Task UpdateBoard_ReturnsBoard_WhenBoardIsUpdatedSuccessfully()
    {
        var board = new Board(title: "Existing Board", icon: "Icon1", theme: "Theme1") { Id = 1 };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        board.Title = "Updated Board";
        
        var result = await _repository.UpdateBoard(board);
        
        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
    }

    [Fact]
    public async Task DeleteBoard_ReturnsBoard_WhenBoardIsDeletedSuccessfully()
    {
        var board = new Board(title: "Board To Delete", icon: "IconDel", theme: "ThemeDel") { Id = 1 };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteBoard(board);
        
        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
    }
}
