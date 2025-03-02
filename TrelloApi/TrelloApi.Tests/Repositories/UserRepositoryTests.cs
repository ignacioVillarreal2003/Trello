using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly IUserRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new UserRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        int userId = 1;
        var user = new User("email@gmail.com", "username", "password") { Id = userId };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(u => u.Id == userId);
        
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }
    
    [Fact]
    public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        int userId = 1;
        
        var result = await _repository.GetAsync(u => u.Id == userId);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
    {
        var email = "email@gmail.com";
        var user = new User(email, "username", "password") { Id = 1 };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(u => u.Email == email);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
    
    [Fact]
    public async Task GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var email = "email@gmail.com";
        
        var result = await _repository.GetAsync(u => u.Email == email);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers_WhenUsersExist()
    {
        var user1 = new User("email1@gmail.com", "username", "password") { Id = 1 };
        var user2 = new User("email2@gmail.com", "username", "password") { Id = 2 };
        
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListAsync();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        var result = await _repository.GetListAsync();
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ShouldReturnUsers_WhenMatchingUsersExist()
    {
        var username = "username";
        var user1 = new User("email1@gmail.com", "username 1", "password") { Id = 1 };
        var user2 = new User("email2@gmail.com", "username 2", "password") { Id = 2 };
        
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListAsync(u => u.Username.Contains(username));
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task GetUsersByUsername_ShouldReturnEmptyList_WhenNoMatchingUsersExist()
    {
        var username = "username";
        
        var result = await _repository.GetListAsync(u => u.Username.Contains(username));
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByCardId_ShouldReturnUsers_WhenCardHasUsers()
    {
        int cardId = 1;
        var user1 = new User("email1@gmail.com", "username", "password") { Id = 1 };
        var user2 = new User("email2@gmail.com", "username", "password") { Id = 2 };
        var userCard1 = new UserCard(1, cardId);
        var userCard2 = new UserCard(2, cardId);
        
        _context.Users.AddRange(user1, user2);
        _context.UserCards.AddRange(userCard1, userCard2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUsersByCardIdAsync(cardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task GetUsersByCardId_ShouldReturnEmptyList_WhenCardHasNoUsers()
    {
        int cardId = 1;
        
        var result = await _repository.GetUsersByCardIdAsync(cardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddUser_ShouldPersistUser_WhenAddedSuccessfully()
    {
        var user = new User("email@gmail.com", "username", "password") { Id = 1 };
        
        await _repository.CreateAsync(user);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
    
    [Fact]
    public async Task UpdateUser_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var user = new User("email@gmail.com", "username", "password") { Id = 1 };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        user.Username = "updated username";
        await _repository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }
    
    [Fact]
    public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
    {
        var user = new User("email@gmail.com", "username", "password") { Id = 1 };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(user);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Users.FindAsync(user.Id);
        
        Assert.Null(result);
    }
}