using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class UserCardServiceTests
{
    private readonly Mock<IUserCardRepository> _mockUserCardRepository;
    private readonly Mock<ILogger<UserCardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UserCardService _service;

    public UserCardServiceTests()
    {
        _mockUserCardRepository = new Mock<IUserCardRepository>();
        _mockLogger = new Mock<ILogger<UserCardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new UserCardService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockUserCardRepository.Object);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnsUsers_WhenUsersFound()
    {
        const int cardId = 1;
        var users = new List<User>
        {
            new User(email: "email1@gmail.com", username: "username 1", password: "password") { Id = 1 },
            new User(email: "email2@gmail.com", username: "username 2", password: "password") { Id = 2 },
        };
        var response = new List<UserResponse>
        {
            new UserResponse { Id = 1, Username = users[0].Username, Email = users[0].Email },
            new UserResponse { Id = 2, Username = users[1].Username, Email = users[1].Email }
        };

        _mockUserCardRepository.Setup(r => r.GetUsersByCardIdAsync(cardId)).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<List<UserResponse>>(users)).Returns(response);

        var result = await _service.GetUsersByCardId(cardId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnsEmptyList_WhenUsersNotFound()
    {
        const int cardId = 1;

        _mockUserCardRepository.Setup(r => r.GetUsersByCardIdAsync(cardId)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns([]);

        var result = await _service.GetUsersByCardId(cardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUserToCard_ShouldReturnsUserCard_WhenAddedSuccessful()
    {
        const int cardId = 1;
        var dto = new AddUserCardDto { UserId = 1 };
        var userCard = new UserCard(dto.UserId, cardId);
        var response = new UserCardResponse { UserId = userCard.UserId, CardId = userCard.CardId };

        _mockUserCardRepository.Setup(r => r.CreateAsync(It.IsAny<UserCard>())).ReturnsAsync(userCard);
        _mockMapper.Setup(m => m.Map<UserCardResponse>(It.IsAny<UserCard>())).Returns(response);

        var result = await _service.AddUserToCard(cardId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveUserFromCard_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int cardId = 1, userId = 2;
        var userCard = new UserCard(userId, cardId);

        _mockUserCardRepository.Setup(r => r
            .GetAsync(It.IsAny<Expression<Func<UserCard, bool>>>())).ReturnsAsync(userCard);
        _mockUserCardRepository.Setup(r => r.DeleteAsync(It.IsAny<UserCard>()));

        var result = await _service.RemoveUserFromCard(userId, cardId);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveUserFromCard_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int cardId = 1, userId = 2;

        _mockUserCardRepository.Setup(r => r
            .GetAsync(It.IsAny<Expression<Func<UserCard, bool>>>())).ReturnsAsync((UserCard?)null);

        var result = await _service.RemoveUserFromCard(userId, cardId);

        Assert.False(result);
    }
}