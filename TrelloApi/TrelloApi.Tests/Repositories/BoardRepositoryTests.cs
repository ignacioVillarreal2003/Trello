using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Repositories;

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
    public async Task GetBoardById_ShouldReturnBoard_WhenBoardExists()
    {
        int boardId = 1;
        var board = new Board(title: "title", background: "background") { Id = boardId };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoardById(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(boardId, result.Id);
    }

    [Fact]
    public async Task GetBoardById_ShouldReturnNull_WhenBoardDoesNotExist()
    {
        int boardId = 1;
        
        var result = await _repository.GetBoardById(boardId);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetBoardById_ShouldReturnNull_WhenBoardIsArchived()
    {
        int boardId = 1;
        var board = new Board(title: "title", background: "background") { Id = boardId, IsArchived = true, ArchivedAt = DateTime.UtcNow };

        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoardById(boardId);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnBoards_WhenUserHasBoards()
    {
        int userId = 1;
        var board1 = new Board(title: "title", background: "background") { Id = 1 };
        var board2 = new Board(title: "title", background: "background") { Id = 2 };
        var userBoard1 = new UserBoard(userId: userId, boardId: 1);
        var userBoard2 = new UserBoard(userId: userId, boardId: 2);

        _context.Boards.AddRange(board1, board2);
        _context.UserBoards.AddRange(userBoard1, userBoard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoardsByUserId(userId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnEmptyList_WhenUserHasNoBoards()
    {
        int userId = 1;
        
        var result = await _repository.GetBoardsByUserId(userId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBoardsByUserId_ShouldNotReturnArchivedBoards()
    {
        int userId = 1;
        var board1 = new Board(title: "title", background: "background") { Id = 1, IsArchived = true, ArchivedAt = DateTime.UtcNow };
        var board2 = new Board(title: "title", background: "background") { Id = 2 };
        var userBoard1 = new UserBoard(userId: userId, boardId: 1);
        var userBoard2 = new UserBoard(userId: userId, boardId: 2);

        _context.Boards.AddRange(board1, board2);
        _context.UserBoards.AddRange(userBoard1, userBoard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetBoardsByUserId(userId);
        
        Assert.NotNull(result);
        Assert.Single(result);
    }
    
    [Fact]
    public async Task AddBoard_ShouldPersistBoard_WhenAddedSuccessfully()
    {
        var board = new Board(title: "title", background: "background") { Id = 1 };
        
        await _repository.AddBoard(board);
        var result = await _context.Boards.FindAsync(board.Id);

        Assert.NotNull(result);
        Assert.Equal(board.Id, result.Id);
    }

    [Fact]
    public async Task UpdateBoard_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var board = new Board(title: "title", background: "background") { Id = 1 };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        board.Title = "updated title";
        await _repository.UpdateBoard(board);
        var result = await _context.Boards.FindAsync(board.Id);

        Assert.NotNull(result);
        Assert.Equal("updated title", result.Title);
    }

    [Fact]
    public async Task DeleteBoard_ShouldRemoveBoard_WhenBoardExists()
    {
        var board = new Board(title: "title", background: "background") { Id = 1 };
        
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteBoard(board);

        var result = await _context.Boards.FindAsync(board.Id);
        
        Assert.Null(result);
    }
}
