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

public class UserCardRepositoryTests
{
    private readonly UserCardRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<UserCardRepository>> _mockLogger;

    public UserCardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<UserCardRepository>>();
        _repository = new UserCardRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetUserCardById_ShouldReturnUserCard_WhenUserCardExists()
    {
        int userId = 1, cardId = 1;
        var userCard = new UserCard(userId: 1, cardId: 1);
        
        _context.UserCards.Add(userCard);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetUserCardById(userId, cardId);
        
        Assert.NotNull(result);
        Assert.Equal(userCard.UserId, result.UserId);
        Assert.Equal(userCard.CardId, result.CardId);
    }

    [Fact]
    public async Task GetUserCardById_ShouldReturnNull_WhenUserCardDoesNotExist()
    {
        int userId = 1, cardId = 1;
        
        var result = await _repository.GetUserCardById(userId, cardId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnUsers_WhenCardHasUsers()
    {
        var cardId = 1;
        var user1 = new User(email: "email1@gmail.com", username: "username", "password") { Id = 1 };
        var user2 = new User(email: "email2@gmail.com", username: "username", "password") { Id = 2 };
        var userCard1 = new UserCard(userId: 1, cardId: 1);
        var userCard2 = new UserCard(userId: 2, cardId: 1);
        
        _context.Users.AddRange(user1, user2);
        _context.UserCards.AddRange(userCard1, userCard2);
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
        
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddUserCard_ShouldPersistUserCard_WhenAddedSuccessfully()
    {
        var userCard = new UserCard(userId: 1, cardId: 1);
        
        await _repository.AddUserCard(userCard);
        var result = await _context.UserCards.FindAsync(userCard.UserId, userCard.CardId);
        
        Assert.NotNull(result);
        Assert.Equal(userCard.UserId, result.UserId);
        Assert.Equal(userCard.CardId, result.CardId);
    }

    [Fact]
    public async Task DeleteUserCard_ShouldRemoveUserCard_WhenUserCardExists()
    {
        var userCard = new UserCard(userId: 1, cardId: 1);

        _context.UserCards.Add(userCard);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteUserCard(userCard);
        var result = await _context.UserCards.FindAsync(userCard.UserId, userCard.CardId);
        
        Assert.Null(result);
    }
}
