using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;                // DTOs: OutputUserDetailsDto, AddUserCardDto, OutputUserCardDetailsDto
using TrelloApi.Domain.Entities;             // Entidades: UserCard, User
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services
{
    public class UserCardServiceTests
    {
        private readonly Mock<IUserCardRepository> _mockUserCardRepository;
        private readonly Mock<ILogger<UserCardService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly UserCardService _service;

        public UserCardServiceTests()
        {
            _mockUserCardRepository = new Mock<IUserCardRepository>();
            _mockLogger = new Mock<ILogger<UserCardService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            _service = new UserCardService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockLogger.Object,
                _mockUserCardRepository.Object);
        }

        [Fact]
        public async Task GetUsersByCardId_ReturnsListOfOutputUserDetailsDto_WhenUsersExist()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var users = new List<User>
            {
                new User(email: "email1@gmail.com", username: "User1", password: "password") { Id = 1},
                new User(email: "email2@gmail.com", username: "User2", password: "password") { Id = 2},
            };
            var outputDtos = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Username = "User1" },
                new OutputUserDetailsDto { Id = 2, Username = "User2" }
            };

            _mockUserCardRepository.Setup(r => r.GetUsersByCardId(cardId))
                .ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(users))
                .Returns(outputDtos);

            // Act
            var result = await _service.GetUsersByCardId(cardId, uid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(outputDtos[0].Id, result[0].Id);
            Assert.Equal(outputDtos[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetUsersByCardId_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            int cardId = 1, uid = 1;
            _mockUserCardRepository.Setup(r => r.GetUsersByCardId(cardId))
                .ReturnsAsync(new List<User>());
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(It.IsAny<List<User>>()))
                .Returns(new List<OutputUserDetailsDto>());

            // Act
            var result = await _service.GetUsersByCardId(cardId, uid);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddUserToCard_ReturnsOutputUserCardDetailsDto_WhenUserCardIsAdded()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var addDto = new AddUserCardDto { UserId = 2 };
            var newUserCard = new UserCard(addDto.UserId, cardId);
            var outputDto = new OutputUserCardDetailsDto 
            { 
                UserId = newUserCard.UserId, 
                CardId = cardId 
            };

            _mockUserCardRepository.Setup(r => r.AddUserCard(It.IsAny<UserCard>()))
                .ReturnsAsync(newUserCard);
            _mockMapper.Setup(m => m.Map<OutputUserCardDetailsDto>(newUserCard))
                .Returns(outputDto);

            // Act
            var result = await _service.AddUserToCard(cardId, addDto, uid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.UserId, result.UserId);
            Assert.Equal(outputDto.CardId, result.CardId);
        }

        [Fact]
        public async Task AddUserToCard_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var addDto = new AddUserCardDto { UserId = 2 };

            _mockUserCardRepository.Setup(r => r.AddUserCard(It.IsAny<UserCard>()))
                .ReturnsAsync((UserCard?)null);

            // Act
            var result = await _service.AddUserToCard(cardId, addDto, uid);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveUserFromCard_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int cardId = 1, uid = 1, userId = 2;
            var existingUserCard = new UserCard(userId, cardId);
            _mockUserCardRepository.Setup(r => r.GetUserCardById(userId, cardId))
                .ReturnsAsync(existingUserCard);
            _mockUserCardRepository.Setup(r => r.DeleteUserCard(existingUserCard))
                .ReturnsAsync(existingUserCard);

            // Act
            var result = await _service.RemoveUserFromCard(userId, cardId, uid);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveUserFromCard_ReturnsFalse_WhenUserCardNotFound()
        {
            // Arrange
            int cardId = 1, uid = 1, userId = 2;
            _mockUserCardRepository.Setup(r => r.GetUserCardById(userId, cardId))
                .ReturnsAsync((UserCard?)null);

            // Act
            var result = await _service.RemoveUserFromCard(userId, cardId, uid);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveUserFromCard_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int cardId = 1, uid = 1, userId = 2;
            var existingUserCard = new UserCard(userId, cardId);
            _mockUserCardRepository.Setup(r => r.GetUserCardById(userId, cardId))
                .ReturnsAsync(existingUserCard);
            _mockUserCardRepository.Setup(r => r.DeleteUserCard(existingUserCard))
                .ReturnsAsync((UserCard?)null);

            // Act
            var result = await _service.RemoveUserFromCard(userId, cardId, uid);

            // Assert
            Assert.False(result);
        }
    }
}
