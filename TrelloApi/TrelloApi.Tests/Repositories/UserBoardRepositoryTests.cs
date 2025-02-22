using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Repositories;
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
    public async Task GetUserBoardById_ShouldReturnUserBoard_WhenUserBoardExists()
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
    public async Task GetUserBoardById_ShouldReturnNull_WhenUserBoardDoesNotExist()
    {
        int userId = 1, boardId = 1;
        
        var result = await _repository.GetUserBoardById(userId, boardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnUsers_WhenBoardHasUsers()
    {
        var boardId = 1;
        var user1 = new User(email: "email1@gmail.com", username: "username", "password") { Id = 1 };
        var user2 = new User(email: "email2@gmail.com", username: "username", "password") { Id = 2 };
        var userBoard1 = new UserBoard(userId: 1, boardId: 1);
        var userBoard2 = new UserBoard(userId: 2, boardId: 1);
        
        _context.Users.AddRange(user1, user2);
        _context.UserBoards.AddRange(userBoard1, userBoard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnEmptyList_WhenBoardHasNoUsers()
    {
        int boardId = 1;
        
        var result = await _repository.GetUsersByBoardId(boardId);
        
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUserBoard_ShouldPersistUserBoard_WhenAddedSuccessfully()
    {
        var userBoard = new UserBoard(userId: 1, boardId: 1);
        
        await _repository.AddUserBoard(userBoard);
        var result = await _context.UserBoards.FindAsync(userBoard.UserId, userBoard.BoardId);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }

    [Fact]
    public async Task DeleteUserBoard_ShouldRemoveUserBoard_WhenUserBoardExists()
    {
        var userBoard = new UserBoard(userId: 1, boardId: 1);

        _context.UserBoards.Add(userBoard);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteUserBoard(userBoard);
        var result = await _context.UserBoards.FindAsync(userBoard.UserId, userBoard.BoardId);

        Assert.Null(result);
    }
}