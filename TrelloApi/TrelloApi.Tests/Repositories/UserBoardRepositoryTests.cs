using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class UserBoardRepositoryTests
{
    private readonly IUserBoardRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UserBoardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new UserBoardRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetUserBoardById_ShouldReturnUserBoard_WhenUserBoardExists()
    {
        int userId = 1, boardId = 1;
        var userBoard = new UserBoard(userId, boardId);
        
        _context.UserBoards.Add(userBoard);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(ub => ub.UserId == userId && ub.BoardId == boardId);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }

    [Fact]
    public async Task GetUserBoardById_ShouldReturnNull_WhenUserBoardDoesNotExist()
    {
        int userId = 1, boardId = 1;
        
        var result = await _repository.GetAsync(ub => ub.UserId == userId && ub.BoardId == boardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnUsers_WhenBoardHasUsers()
    {
        int boardId = 1;
        var user1 = new User("email1@gmail.com", "username", "password") { Id = 1 };
        var user2 = new User("email2@gmail.com", "username", "password") { Id = 2 };
        var userBoard1 = new UserBoard(1, boardId);
        var userBoard2 = new UserBoard(2, boardId);
        
        _context.Users.AddRange(user1, user2);
        _context.UserBoards.AddRange(userBoard1, userBoard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByBoardIdAsync(boardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnEmptyList_WhenBoardHasNoUsers()
    {
        int boardId = 1;
        
        var result = await _repository.GetUsersByBoardIdAsync(boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUserBoard_ShouldPersistUserBoard_WhenAddedSuccessfully()
    {
        var userBoard = new UserBoard(1, 1);
        
        await _repository.CreateAsync(userBoard);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.UserBoards.FindAsync(userBoard.UserId, userBoard.BoardId);
        
        Assert.NotNull(result);
        Assert.Equal(userBoard.UserId, result.UserId);
        Assert.Equal(userBoard.BoardId, result.BoardId);
    }

    [Fact]
    public async Task DeleteUserBoard_ShouldRemoveUserBoard_WhenUserBoardExists()
    {
        var userBoard = new UserBoard(1, 1);

        _context.UserBoards.Add(userBoard);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(userBoard);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.UserBoards.FindAsync(userBoard.UserId, userBoard.BoardId);
        Assert.Null(result);
    }
}