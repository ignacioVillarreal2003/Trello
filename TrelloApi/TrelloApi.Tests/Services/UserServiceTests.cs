using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Services;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;          // Contiene RegisterUserDto, LoginUserDto, UpdateUserDto, OutputUserDetailsDto, etc.
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly Mock<IEncrypt> _mockEncrypt;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();
            _mockEncrypt = new Mock<IEncrypt>();

            _service = new UserService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockUserRepository.Object,
                _mockLogger.Object,
                _mockEncrypt.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithFullList()
        {
            // Arrange
            var userId = 1;
            var listUser = new List<User>
            {
                new User("user1@example.com", "user1", "password1", "Light"),
                new User("user2@example.com", "user2", "password2", "Light"),
            };
            var listOutputUserDto = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
            };

            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsers(userId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var userId = 1;
            var listUser = new List<User>();
            var listOutputUserDto = new List<OutputUserDetailsDto>();

            _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsers(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsersByUsername_ReturnsOk_WithFullList()
        {
            // Arrange
            var username = "";
            var userId = 1;
            var listUser = new List<User>
            {
                new User("user1@example.com", "user1", "password1", "Light"),
                new User("user2@example.com", "user2", "password2", "Light"),
            };
            var listOutputUserDto = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
            };

            _mockUserRepository.Setup(r => r.GetUsersByUsername(username)).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsersByUsername(username, userId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsersByUsername_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var username = "";
            var userId = 1;
            var listUser = new List<User>();
            var listOutputUserDto = new List<OutputUserDetailsDto>();

            _mockUserRepository.Setup(r => r.GetUsersByUsername(username)).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsersByUsername(username, userId);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public async Task GetUsersByCardId_ReturnsOk_WithFullList()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var listUser = new List<User>
            {
                new User("user1@example.com", "user1", "password1", "Light"),
                new User("user2@example.com", "user2", "password2", "Light"),
            };
            var listOutputUserDto = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
            };

            _mockUserRepository.Setup(r => r.GetUsersByCardId(taskId)).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsersByCardId(userId, taskId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsersByCardId_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;
            var listUser = new List<User>();
            var listOutputUserDto = new List<OutputUserDetailsDto>();

            _mockUserRepository.Setup(r => r.GetUsersByCardId(taskId)).ReturnsAsync(listUser);
            _mockMapper.Setup(m => m.Map<List<OutputUserDetailsDto>>(listUser))
                       .Returns(listOutputUserDto);

            // Act
            var result = await _service.GetUsersByCardId(userId, taskId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task RegisterUser_ReturnsOk_WithElementCreated()
        {
            // Arrange
            var registerUserDto = new RegisterUserDto 
            { 
                Email = "user1@example.com", 
                Username = "user1", 
                Password = "password1" 
            };
            var hashedPassword = "hashed_password1"; 
            var newUser = new User(registerUserDto.Email, registerUserDto.Username, hashedPassword, "Light") { Id = 1 };
            var outputDto = new OutputUserDetailsDto 
            { 
                Id = 1, 
                Email = registerUserDto.Email, 
                Username = registerUserDto.Username, 
                Theme = "Light" 
            };

            _mockEncrypt.Setup(e => e.HashPassword(registerUserDto.Password))
                        .Returns(hashedPassword);
            _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>()))
                               .ReturnsAsync(newUser);
            _mockMapper.Setup(m => m.Map<OutputUserDetailsDto>(newUser))
                       .Returns(outputDto);

            // Act
            var result = await _service.RegisterUser(registerUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Email, result.Email);
            Assert.Equal(outputDto.Username, result.Username);
            Assert.Equal(outputDto.Theme, result.Theme);
        }

        [Fact]
        public async Task RegisterUser_ReturnsNull_WithElementNotCreated()
        {
            // Arrange
            var registerUserDto = new RegisterUserDto 
            { 
                Email = "user1@example.com", 
                Username = "user1", 
                Password = "password1" 
            };
            var hashedPassword = "hashed_password1"; 
            User? newUser = null;

            _mockEncrypt.Setup(e => e.HashPassword(registerUserDto.Password))
                        .Returns(hashedPassword);
            _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>()))
                               .ReturnsAsync(newUser);

            // Act
            var result = await _service.RegisterUser(registerUserDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUser_ReturnsOk_WithElementLogged()
        {
            // Arrange
            var loginUserDto = new LoginUserDto 
            { 
                Email = "user1@example.com", 
                Password = "password1" 
            };
            var user = new User("user1@example.com", "user1", "password1", "Light") { Id = 1 };
            var outputDto = new OutputUserDetailsDto 
            { 
                Id = 1, 
                Email = "user1@example.com", 
                Username = "user1", 
                Theme = "Light" 
            };

            _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email))
                               .ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(loginUserDto.Password, user.Password))
                        .Returns(true);
            _mockMapper.Setup(m => m.Map<OutputUserDetailsDto>(user))
                       .Returns(outputDto);

            // Act
            var result = await _service.LoginUser(loginUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Email, result.Email);
            Assert.Equal(outputDto.Username, result.Username);
            Assert.Equal(outputDto.Theme, result.Theme);
        }

        [Fact]
        public async Task LoginUser_ReturnsNull_WithElementNotLogged()
        {
            // Arrange
            var loginUserDto = new LoginUserDto 
            { 
                Email = "user1@example.com", 
                Password = "password1" 
            };
            User? user = null;

            _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email))
                               .ReturnsAsync(user);

            // Act
            var result = await _service.LoginUser(loginUserDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUser_ThrowsUnauthorizedAccessException_WithInvalidPassword()
        {
            // Arrange
            var loginUserDto = new LoginUserDto 
            { 
                Email = "user1@example.com", 
                Password = "wrongpassword" 
            };
            var user = new User("user1@example.com", "user1", "password1", "Light") { Id = 1 };

            _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email))
                               .ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(loginUserDto.Password, user.Password))
                        .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginUser(loginUserDto));
            Assert.Equal("Invalid user credentials.", exception.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_WithElementUpdated()
        {
            // Arrange
            var userId = 1;
            var user = new User("user1@example.com", "oldUsername", "password1", "Dark") { Id = userId };
            var updateUserDto = new UpdateUserDto 
            { 
                Username = "newUsername", 
                Theme = "Light" 
            };
            var updatedUser = new User("user1@example.com", "newUsername", "password1", "Light") { Id = userId };
            var outputDto = new OutputUserDetailsDto 
            { 
                Id = userId, 
                Email = "user1@example.com", 
                Username = "newUsername", 
                Theme = "Light" 
            };

            _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(updatedUser);
            _mockMapper.Setup(m => m.Map<OutputUserDetailsDto>(updatedUser)).Returns(outputDto);

            // Act
            var result = await _service.UpdateUser(updateUserDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Email, result.Email);
            Assert.Equal(outputDto.Username, result.Username);
            Assert.Equal(outputDto.Theme, result.Theme);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var userId = 1;
            var updateUserDto = new UpdateUserDto 
            { 
                Username = "newUsername", 
                Theme = "Light" 
            };

            _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.UpdateUser(updateUserDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUser_ThrowsException_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            var userId = 1;
            var user = new User("user1@example.com", "oldUsername", "password1", "Dark") { Id = userId };
            var updateUserDto = new UpdateUserDto 
            { 
                OldPassword = "wrongPassword", 
                NewPassword = "newPassword123" 
            };

            _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(updateUserDto.OldPassword, user.Password))
                        .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateUser(updateUserDto, userId));
        }

        [Fact]
        public async Task DeleteUser_ReturnsTrue_WithElementDeleted()
        {
            // Arrange
            var userId = 1;
            var user = new User("user1@example.com", "username", "password1", "Dark") { Id = userId };
            var deletedUser = new User("user1@example.com", "username", "password1", "Dark") { Id = userId };
            var outputDto = new OutputUserDetailsDto 
            { 
                Id = userId, 
                Email = "user1@example.com", 
                Username = "username", 
                Theme = "Dark" 
            };

            _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.DeleteUser(It.IsAny<User>())).ReturnsAsync(deletedUser);
            _mockMapper.Setup(m => m.Map<OutputUserDetailsDto>(deletedUser)).Returns(outputDto);

            // Act
            var result = await _service.DeleteUser(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsFalse_WhenUserNotFound()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.DeleteUser(userId);

            // Assert
            Assert.False(result);
        }
    }
}