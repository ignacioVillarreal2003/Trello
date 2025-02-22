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
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        int userId = 1;
        var user = new User(email: "email@gmail.com", username: "username", password: "password" ) { Id = userId };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserById(userId);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        int userId = 1;
        
        var result = await _repository.GetUserById(userId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
    {
        var email = "email@gmail.com";
        var user = new User(email: email, username: "username", password: "password" ) { Id = 1 };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserByEmail(email);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var email = "email@gmail.com";
        
        var result = await _repository.GetUserByEmail(email);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers_WhenUsersExist()
    {
        var user1 = new User(email: "email1@gmail.com", username: "username", password: "password" ) { Id = 1 };
        var user2 = new User(email: "email2@gmail.com", username: "username", password: "password" ) { Id = 2 };

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsers();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        var result = await _repository.GetUsers();
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ShouldReturnUsers_WhenMatchingUsersExist()
    {
        var username = "username";
        var user1 = new User(email: "email1@gmail.com", username: "username 1", password: "password" ) { Id = 1 };
        var user2 = new User(email: "email2@gmail.com", username: "username 2", password: "password" ) { Id = 2 };

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByUsername(username);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByUsername_ShouldReturnEmptyList_WhenNoMatchingUsersExist()
    {
        var username = "username";
        
        var result = await _repository.GetUsersByUsername(username);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByCardId_ShouldReturnUsers_WhenCardHasUsers()
    {
        int cardId = 1;
        var user1 = new User(email: "email1@gmail.com", username: "username", password: "password" ) { Id = 1 };
        var user2 = new User(email: "email2@gmail.com", username: "username", password: "password" ) { Id = 2 };
        var userTask1 = new UserCard(userId: 1, cardId: 1);
        var userTask2 = new UserCard(userId: 2, cardId: 1);

        _context.Users.AddRange(user1, user2);
        _context.UserCards.AddRange(userTask1, userTask2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByCardId(cardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnEmptyList_WhenCardHasNoUsers()
    {
        int cardId = 1;
        
        var result = await _repository.GetUsersByCardId(cardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUser_ShouldPersistUser_WhenAddedSuccessfully()
    {
        var user = new User(email: "email@gmail.com", username: "username", password: "password" ) { Id = 1 };
        
        await _repository.AddUser(user);
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task UpdateUser_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var user = new User(email: "email@gmail.com", username: "username", password: "password" ) { Id = 1 };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        user.Username = "updated username";
        await _repository.UpdateUser(user);
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
    {
        var user = new User(email: "email@gmail.com", username: "username", password: "password" ) { Id = 1 };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteUser(user);
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.Null(result);
    }
}
