using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEncrypt> _mockEncrypt;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEncrypt = new Mock<IEncrypt>();

            _service = new UserService(
                _mockMapper.Object,
                _mockUnitOfWork.Object,
                _mockUserRepository.Object,
                _mockLogger.Object,
                _mockEncrypt.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithFullList()
        {
            var users = new List<User>
            {
                new User("user1@example.com", "username 1", "password", "theme"),
                new User("user2@example.com", "username 2", "password", "theme"),
            };
            var response = new List<UserResponse>
            {
                new UserResponse { Email = users[0].Email, Username = users[0].Username, Theme = users[0].Theme },
                new UserResponse { Email = users[1].Email, Username = users[1].Username, Theme = users[1].Theme },
            };

            _mockUserRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns(response);

            var result = await _service.GetUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsers_ReturnsOk_WithEmptyList()
        {
            _mockUserRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync([]);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns([]);

            var result = await _service.GetUsers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsersByUsername_ShouldReturnsUsers_WhenUsersFound()
        {
            const string username = "";
            var users = new List<User>
            {
                new User("user1@example.com", "username 1", "password", "theme"),
                new User("user2@example.com", "username 2", "password", "theme"),
            };
            var response = new List<UserResponse>
            {
                new UserResponse { Email = users[0].Email, Username = users[0].Username, Theme = users[0].Theme },
                new UserResponse { Email = users[1].Email, Username = users[1].Username, Theme = users[1].Theme },
            };

            _mockUserRepository.Setup(r => r.GetUsersByUsernameAsync(username)).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns(response);

            var result = await _service.GetUsersByUsername(username);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsersByUsername_ShouldReturnsEmptyList_WhenUsersNotFound()
        {
            const string username = "";

            _mockUserRepository.Setup(r => r.GetUsersByUsernameAsync(username)).ReturnsAsync([]);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns([]);

            var result = await _service.GetUsersByUsername(username);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetUsersByCardId_ShouldReturnsUsers_WhenUsersFound()
        {
            const int taskId = 1;
            var users = new List<User>
            {
                new User("user1@example.com", "username", "password", "theme"),
                new User("user2@example.com", "username", "password", "theme"),
            };
            var response = new List<UserResponse>
            {
                new UserResponse { Email = users[0].Email, Username = users[0].Username, Theme = users[0].Theme },
                new UserResponse { Email = users[1].Email, Username = users[1].Username, Theme = users[1].Theme },
            };

            _mockUserRepository.Setup(r => r.GetUsersByCardIdAsync(taskId)).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns(response);

            var result = await _service.GetUsersByCardId(taskId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUsersByCardId_ShouldReturnsEmptyList_WhenUsersNotFound()
        {
            const int taskId = 1;

            _mockUserRepository.Setup(r => r.GetUsersByCardIdAsync(taskId)).ReturnsAsync([]);
            _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns([]);

            var result = await _service.GetUsersByCardId(taskId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task RegisterUser_ShouldReturnsUser_WhenRegisteredSuccessful()
        {
            const string hashedPassword = "hashed_password"; 
            var dto = new RegisterUserDto { Email = "user@email.com", Username = "username", Password = "password" };
            var response = new UserResponse { Email = dto.Email, Username = dto.Username };

            _mockEncrypt.Setup(e => e.HashPassword(dto.Password)).Returns(hashedPassword);
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()));
            _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(response);
            
            var result = await _service.RegisterUser(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnsUser_WhenLoggedSuccessful()
        {
            var dto = new LoginUserDto { Email = "user@email.com", Password = "password" };
            var user = new User(email: dto.Email, username: "username", password: dto.Password, theme: "theme") { Id = 1 };
            var response = new UserResponse { Id = user.Id, Email = user.Email, Username = user.Username, Theme = user.Theme };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(dto.Password, user.Password)).Returns(true);
            _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            var result = await _service.LoginUser(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnsNull_WhenLoggedUnsuccessful()
        {
            var dto = new LoginUserDto { Email = "user@email.com", Password = "password" };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync((User?)null);

            var result = await _service.LoginUser(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUser_ShouldThrowsException_WhenInvalidPassword()
        {
            var dto = new LoginUserDto { Email = "user@email.com", Password = "wrong password" };
            var user = new User(email: dto.Email, username: "username", password: "password", theme: "theme") { Id = 1 };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(dto.Password, user.Password)).Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginUser(dto));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnsUser_WhenUpdatedSuccessful()
        {
            const int userId = 1;
            var user = new User("user@example.com", "username", "password", "theme") { Id = userId };
            var dto = new UpdateUserDto { Username = "updated username" };
            var response = new UserResponse { Id = userId, Email = user.Email, Username = dto.Username, Theme = user.Theme };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()));
            _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            var result = await _service.UpdateUser(dto, userId);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnsNull_WhenUpdatedUnsuccessful()
        {
            var userId = 1;
            var dto = new UpdateUserDto { Username = "updated username" };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync((User?)null);

            var result = await _service.UpdateUser(dto, userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldThrowsException_WhenOldPasswordIsIncorrect()
        {
            const int userId = 1;
            var dto = new UpdateUserDto { OldPassword = "old password", NewPassword = "new password" };
            var user = new User("user@example.com", "username", "password", "theme") { Id = userId };
            

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync(user);
            _mockEncrypt.Setup(e => e.ComparePassword(dto.OldPassword, user.Password)).Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateUser(dto, userId));
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnsTrue_WhenDeletedSuccessful()
        {
            const int userId = 1;
            var user = new User("user@example.com", "username", "password", "theme") { Id = userId };
            var response = new UserResponse { Id = userId, Email = user.Email, Username = user.Username, Theme = user.Theme };

            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.DeleteAsync(It.IsAny<User>()));
            _mockMapper.Setup(m => m.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            var result = await _service.DeleteUser(userId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnsFalse_WhenDeletedUnsuccessfully()
        {
            const int userId = 1;
            
            _mockUserRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<User,bool>>>())).ReturnsAsync((User?)null);

            var result = await _service.DeleteUser(userId);

            Assert.False(result);
        }
    }
}