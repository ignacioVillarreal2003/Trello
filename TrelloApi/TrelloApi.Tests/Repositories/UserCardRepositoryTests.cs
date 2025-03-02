using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class UserCardRepositoryTests
{
    private readonly IUserCardRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UserCardRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new UserCardRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetUserCardById_ShouldReturnUserCard_WhenUserCardExists()
    {
        int userId = 1, cardId = 1;
        var userCard = new UserCard(userId, cardId);
        
        _context.UserCards.Add(userCard);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(uc => uc.UserId == userId && uc.CardId == cardId);
        
        Assert.NotNull(result);
        Assert.Equal(userCard.UserId, result.UserId);
        Assert.Equal(userCard.CardId, result.CardId);
    }

    [Fact]
    public async Task GetUserCardById_ShouldReturnNull_WhenUserCardDoesNotExist()
    {
        int userId = 1, cardId = 1;
        
        var result = await _repository.GetAsync(uc => uc.UserId == userId && uc.CardId == cardId);
        
        Assert.Null(result);
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
    public async Task AddUserCard_ShouldPersistUserCard_WhenAddedSuccessfully()
    {
        var userCard = new UserCard(1, 1);
        
        await _repository.CreateAsync(userCard);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.UserCards.FindAsync(userCard.UserId, userCard.CardId);
        
        Assert.NotNull(result);
        Assert.Equal(userCard.UserId, result.UserId);
        Assert.Equal(userCard.CardId, result.CardId);
    }

    [Fact]
    public async Task DeleteUserCard_ShouldRemoveUserCard_WhenUserCardExists()
    {
        var userCard = new UserCard(1, 1);

        _context.UserCards.Add(userCard);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(userCard);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.UserCards.FindAsync(userCard.UserId, userCard.CardId);
        
        Assert.Null(result);
    }
}