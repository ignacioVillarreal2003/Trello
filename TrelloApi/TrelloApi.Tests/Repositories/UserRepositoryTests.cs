using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly UserRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        int userId = 1;
        var user = new User(email: "Email1@gmail.com", username: "Username", password: "Password" ) { Id = userId };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserById(userId);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
    {
        int userId = 1;
        
        var result = await _repository.GetUserById(userId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmail_ReturnsUser_WhenUserExists()
    {
        var email = "Email@gmail.com";
        var user = new User(email: email, username: "Username", password: "Password" ) { Id = 1 };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserByEmail(email);
        
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetUserByEmail_ReturnsNull_WhenUserDoesNotExist()
    {
        var email = "Email@gmail.com";
        
        var result = await _repository.GetUserByEmail(email);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUsers_ReturnsUsers_WhenUsersExistForUser()
    {
        var user1 = new User(email: "Email1@gmail.com", username: "Username", password: "Password" ) { Id = 1 };
        var user2 = new User(email: "Email2@gmail.com", username: "Username", password: "Password" ) { Id = 2 };

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsers();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsers_ReturnsEmptyList_WhenNoUsersExistForUser()
    {
        var result = await _repository.GetUsers();
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ReturnsUsers_WhenUsersExistForUser()
    {
        var username = "";
        var user1 = new User(email: "Email1@gmail.com", username: "Username", password: "Password" ) { Id = 1 };
        var user2 = new User(email: "Email2@gmail.com", username: "Username", password: "Password" ) { Id = 2 };

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByUsername(username);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByUsername_ReturnsEmptyList_WhenNoUsersExistForUser()
    {
        var username = "";
        
        var result = await _repository.GetUsersByUsername(username);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByBoardId_ReturnsUsers_WhenUsersExistForUser()
    {
        int boardId = 1;
        var user1 = new User(email: "Email1@gmail.com", username: "Username", password: "Password" ) { Id = 1 };
        var user2 = new User(email: "Email2@gmail.com", username: "Username", password: "Password" ) { Id = 2 };
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
    public async Task GetUsersByBoardId_ReturnsEmptyList_WhenNoUsersExistForUser()
    {
        int boardId = 1;
        
        var result = await _repository.GetUsersByBoardId(boardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByTaskId_ReturnsUsers_WhenUsersExistForUser()
    {
        int taskId = 1;
        var user1 = new User(email: "Email1@gmail.com", username: "Username", password: "Password" ) { Id = 1 };
        var user2 = new User(email: "Email2@gmail.com", username: "Username", password: "Password" ) { Id = 2 };
        var userTask1 = new UserCard(userId: 1, taskId: 1);
        var userTask2 = new UserCard(userId: 2, taskId: 1);

        _context.Users.AddRange(user1, user2);
        _context.UserTasks.AddRange(userTask1, userTask2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByTaskId_ReturnsEmptyList_WhenNoUsersExistForUser()
    {
        int taskId = 1;
        
        var result = await _repository.GetUsersByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUser_ReturnsUser_WhenUserIsAddedSuccessfully()
    {
        var user = new User(email: "Email@gmail.com", username: "Username", password: "Password" ) { Id = 1 };
        
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddUser(user);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task UpdateUser_ReturnsUser_WhenUserIsUpdatedSuccessfully()
    {
        var user = new User(email: "Email@gmail.com", username: "Username", password: "Password" ) { Id = 1 };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        user.Username = "Updated Username";
        
        var result = await _repository.UpdateUser(user);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task DeleteUser_ReturnsUser_WhenUserIsDeletedSuccessfully()
    {
        var user = new User(email: "Email@gmail.com", username: "Username", password: "Password" ) { Id = 1 };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteUser(user);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
}
