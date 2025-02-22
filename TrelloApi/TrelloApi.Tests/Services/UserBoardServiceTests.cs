using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Constants;           // Para RoleValues, si es necesario
using TrelloApi.Domain.DTOs;
using TrelloApi.Infrastructure.Persistence.Interfaces; // Aquí se encuentran AddUserBoardDto y OutputUserBoardDetailsDto
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services
{
    public class UserBoardServiceTests
    {
        private readonly Mock<IUserBoardRepository> _mockUserBoardRepository;
        private readonly Mock<ILogger<UserBoardService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly UserBoardService _service;

        public UserBoardServiceTests()
        {
            _mockUserBoardRepository = new Mock<IUserBoardRepository>();
            _mockLogger = new Mock<ILogger<UserBoardService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            // Importante: el orden de parámetros debe coincidir con el constructor del servicio.
            _service = new UserBoardService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockUserBoardRepository.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserBoardById_ReturnsOutputUserBoardDetailsDto_WhenUserBoardExists()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var userBoard = new UserBoard(userId: userId, boardId: boardId); // Se asume que este constructor existe
            var outputDto = new OutputUserBoardDetailsDto 
            { 
                UserId = userBoard.UserId, 
                BoardId = userBoard.BoardId 
            };

            _mockUserBoardRepository
                .Setup(r => r.GetUserBoardById(userId, boardId))
                .ReturnsAsync(userBoard);
            _mockMapper
                .Setup(m => m.Map<OutputUserBoardDetailsDto>(userBoard))
                .Returns(outputDto);

            // Act
            var result = await _service.GetUsersByBoardId(boardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserBoardById_ReturnsNull_WhenUserBoardDoesNotExist()
        {
            // Arrange
            int boardId = 1, userId = 1;
            _mockUserBoardRepository
                .Setup(r => r.GetUserBoardById(userId, boardId))
                .ReturnsAsync((UserBoard?)null);

            // Act
            var result = await _service.GetUsersByBoardId(boardId, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserBoard_ReturnsOutputUserBoardDetailsDto_WhenUserBoardIsAdded()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addDto = new AddUserBoardDto 
            { 
                UserId = userId, 
                Role = "Member" // Se asume que el DTO tiene la propiedad Role
            };
            var newUserBoard = new UserBoard(userId: addDto.UserId, boardId: boardId, role: addDto.Role);
            var outputDto = new OutputUserBoardDetailsDto 
            { 
                UserId = newUserBoard.UserId, 
                BoardId = newUserBoard.BoardId, 
                Role = newUserBoard.Role 
            };

            _mockUserBoardRepository
                .Setup(r => r.AddUserBoard(It.IsAny<UserBoard>()))
                .ReturnsAsync(newUserBoard);
            _mockMapper
                .Setup(m => m.Map<OutputUserBoardDetailsDto>(newUserBoard))
                .Returns(outputDto);

            // Act
            var result = await _service.AddUserToBoard(boardId, addDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.UserId, result.UserId);
            Assert.Equal(outputDto.BoardId, result.BoardId);
            Assert.Equal(outputDto.Role, result.Role);
        }

        [Fact]
        public async Task AddUserBoard_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addDto = new AddUserBoardDto 
            { 
                UserId = userId, 
                Role = "Member" 
            };

            _mockUserBoardRepository
                .Setup(r => r.AddUserBoard(It.IsAny<UserBoard>()))
                .ReturnsAsync((UserBoard?)null);

            // Act
            var result = await _service.AddUserToBoard(boardId, addDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveUserFromBoard_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int boardId = 1, uid = 1, userToDeleteId = 2;
            var existingUserBoard = new UserBoard(userId: userToDeleteId, boardId: boardId, role: "Member");
            _mockUserBoardRepository
                .Setup(r => r.GetUserBoardById(userToDeleteId, boardId))
                .ReturnsAsync(existingUserBoard);
            _mockUserBoardRepository
                .Setup(r => r.DeleteUserBoard(existingUserBoard))
                .ReturnsAsync(existingUserBoard);

            // Act
            var result = await _service.RemoveUserFromBoard(boardId, userToDeleteId, uid);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveUserFromBoard_ReturnsFalse_WhenUserBoardNotFound()
        {
            // Arrange
            int boardId = 1, uid = 1, userToDeleteId = 2;
            _mockUserBoardRepository
                .Setup(r => r.GetUserBoardById(userToDeleteId, boardId))
                .ReturnsAsync((UserBoard?)null);

            // Act
            var result = await _service.RemoveUserFromBoard(boardId, userToDeleteId, uid);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveUserFromBoard_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int boardId = 1, uid = 1, userToDeleteId = 2;
            var existingUserBoard = new UserBoard(userId: userToDeleteId, boardId: boardId, role: "Member");
            _mockUserBoardRepository
                .Setup(r => r.GetUserBoardById(userToDeleteId, boardId))
                .ReturnsAsync(existingUserBoard);
            _mockUserBoardRepository
                .Setup(r => r.DeleteUserBoard(existingUserBoard))
                .ReturnsAsync((UserBoard?)null);

            // Act
            var result = await _service.RemoveUserFromBoard(boardId, userToDeleteId, uid);

            // Assert
            Assert.False(result);
        }
    }
}
