using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class UserBoardRepositoryTests
{
    private readonly UserBoardRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<UserBoardRepository>> _mockLogger;

    public UserBoardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<UserBoardRepository>>();
        _repository = new UserBoardRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetUserBoardById_ReturnsUserBoard_WhenUserBoardExists()
    {
        int userId = 1, boardId = 1;
        var userBoard = new UserBoard(userId: 1, boardId: 1);
        
        _context.UserBoards.Add(userBoard);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserBoardById(userId, boardId);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }

    [Fact]
    public async Task GetUserBoardById_ReturnsNull_WhenUserBoardDoesNotExist()
    {
        int userId = 1, boardId = 1;
        
        var result = await _repository.GetUserBoardById(userId, boardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task AddUserBoard_ReturnsUserBoard_WhenUserBoardIsAddedSuccessfully()
    {
        var userBoard = new UserBoard(userId: 1, boardId: 1);

        _context.UserBoards.RemoveRange(_context.UserBoards);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddUserBoard(userBoard);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }

    [Fact]
    public async Task DeleteUserBoard_ReturnsUserBoard_WhenUserBoardIsDeletedSuccessfully()
    {
        var userBoard = new UserBoard(userId: 1, boardId: 1);

        _context.UserBoards.Add(userBoard);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteUserBoard(userBoard);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }
}