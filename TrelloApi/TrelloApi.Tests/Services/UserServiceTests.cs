using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.User.DTO;

namespace TrelloApi.Tests.Services;

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
        
        _service = new UserService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockUserRepository.Object, _mockLogger.Object, _mockEncrypt.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsOk_WithFullList()
    {
        var userId = 1;
        var listUser = new List<User>
        {
            new User("user1@example.com", "user1", "password1", "Light"),
            new User("user2@example.com", "user2", "password2", "Light"),
        };
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
        };

        _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsers(userId);

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk_WithEmptyList()
    {
        var userId = 1;
        var listUser = new List<User>();
        var listOutputUserDto = new List<OutputUserDto>();

        _mockUserRepository.Setup(r => r.GetUsers()).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsers(userId);

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ReturnsOk_WithFullList()
    {
        var username = "";
        var userId = 1;
        var listUser = new List<User>
        {
            new User("user1@example.com", "user1", "password1", "Light"),
            new User("user2@example.com", "user2", "password2", "Light"),
        };
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
        };

        _mockUserRepository.Setup(r => r.GetUsersByUsername(username)).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByUsername(userId, username);

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ReturnsOk_WithEmptyList()
    {
        var username = "";
        var userId = 1;
        var listUser = new List<User>();
        var listOutputUserDto = new List<OutputUserDto>();

        _mockUserRepository.Setup(r => r.GetUsersByUsername("")).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByUsername(userId, username);

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByBoardId_ReturnsOk_WithFullList()
    {
        var boardId = 1;
        var userId = 1;
        var listUser = new List<User>
        {
            new User("user1@example.com", "user1", "password1", "Light"),
            new User("user2@example.com", "user2", "password2", "Light"),
        };
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
        };

        _mockUserRepository.Setup(r => r.GetUsersByBoardId(boardId)).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByBoardId(userId, boardId);

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetUsersByBoardId_ReturnsOk_WithEmptyList()
    {
        var boardId = 1;
        var userId = 1;
        var listUser = new List<User>();
        var listOutputUserDto = new List<OutputUserDto>();

        _mockUserRepository.Setup(r => r.GetUsersByBoardId(boardId)).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByBoardId(userId, boardId);

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByTaskId_ReturnsOk_WithFullList()
    {
        var taskId = 1;
        var userId = 1;
        var listUser = new List<User>
        {
            new User("user1@example.com", "user1", "password1", "Light"),
            new User("user2@example.com", "user2", "password2", "Light"),
        };
        var listOutputUserDto = new List<OutputUserDto>
        {
            new OutputUserDto { Email = "user1@example.com", Username = "user1", Theme = "Light" },
            new OutputUserDto { Email = "user2@example.com", Username = "user2", Theme = "Light" },
        };

        _mockUserRepository.Setup(r => r.GetUsersByTaskId(taskId)).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByTaskId(userId, taskId);

        Assert.Equal(2, result.Count);
    }
    
    [Fact]
    public async Task GetUsersByTaskId_ReturnsOk_WithEmptyList()
    {
        var taskId = 1;
        var userId = 1;
        var listUser = new List<User>();
        var listOutputUserDto = new List<OutputUserDto>();

        _mockUserRepository.Setup(r => r.GetUsersByTaskId(taskId)).ReturnsAsync(listUser);
        _mockMapper.Setup(m => m.Map<List<OutputUserDto>>(listUser)).Returns(listOutputUserDto);

        var result = await _service.GetUsersByTaskId(userId, taskId);

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task RegisterUser_ReturnsOk_WithElementCreated()
    {
        var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
        var hashedPassword = "hashed_password1"; 
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" };

        _mockEncrypt.Setup(e => e.HashPassword(registerUserDto.Password)).Returns(hashedPassword);
        _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _mockMapper.Setup(m => m.Map<OutputUserDto>(It.IsAny<User>())).Returns(outputUserDto);

        var result = await _service.RegisterUser(registerUserDto);

        Assert.NotNull(result);
        Assert.Equal(outputUserDto.Id, result.Id);
        Assert.Equal(outputUserDto.Email, result.Email);
        Assert.Equal(outputUserDto.Username, result.Username);
        Assert.Equal(outputUserDto.Theme, result.Theme);
    }

    
    [Fact]
    public async Task RegisterUser_ReturnsNull_WithElementNotCreated()
    {
        var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
        var hashedPassword = "hashed_password1"; 
        var user = (User?)null;

        _mockEncrypt.Setup(e => e.HashPassword(registerUserDto.Password)).Returns(hashedPassword);
        _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync(user);

        var result = await _service.RegisterUser(registerUserDto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task LoginUser_ReturnsOk_WithElementLogged()
    {
        var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
        var user = new User("user1@example.com", "user1", "password1");
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light"};

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email)).ReturnsAsync(user);
        _mockEncrypt.Setup(e => e.ComparePassword(loginUserDto.Password, user.Password)).Returns(true);
        _mockMapper.Setup(m => m.Map<OutputUserDto>(user)).Returns(outputUserDto);

        var result = await _service.LoginUser(loginUserDto);

        Assert.NotNull(result);
        Assert.Equal(outputUserDto.Id, result.Id);
        Assert.Equal(outputUserDto.Email, result.Email);
        Assert.Equal(outputUserDto.Username, result.Username);
        Assert.Equal(outputUserDto.Theme, result.Theme);
    }
    
    [Fact]
    public async Task LoginUser_ReturnsNull_WithElementNotLogged()
    {
        var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
        var user = (User?)null;

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email)).ReturnsAsync(user);

        var result = await _service.LoginUser(loginUserDto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task LoginUser_ThrowsUnauthorizedAccessException_WithInvalidPassword()
    {
        var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "wrongpassword" };
        var user = new User("user1@example.com", "user1", "password1");

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginUserDto.Email)).ReturnsAsync(user);
        _mockEncrypt.Setup(e => e.ComparePassword(loginUserDto.Password, user.Password)).Returns(false);
        
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginUser(loginUserDto));

        Assert.Equal("Invalid user credentials.", exception.Message);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsOk_WithElementUpdated()
    {
        var userId = 1;
        var user = new User("user1@example.com", "oldUsername", "password1", "Dark");
        var updateUserDto = new UpdateUserDto { Username = "newUsername", Theme = "Light" };
        var updatedUser = new User("user1@example.com", "newUsername", "password1", "Light"); 
        var outputUserDto = new OutputUserDto { Id = 1, Email = "user1@example.com", Username = "newUsername", Theme = "Light" };

        _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(updatedUser);
        _mockMapper.Setup(m => m.Map<OutputUserDto>(updatedUser)).Returns(outputUserDto);

        var result = await _service.UpdateUser(userId, updateUserDto);

        Assert.NotNull(result);
        Assert.Equal(outputUserDto.Id, result.Id);
        Assert.Equal(outputUserDto.Email, result.Email);
        Assert.Equal(outputUserDto.Username, result.Username);
        Assert.Equal(outputUserDto.Theme, result.Theme);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNull_WhenUserNotFound()
    {
        var userId = 1;
        var updateUserDto = new UpdateUserDto { Username = "newUsername", Theme = "Light" };

        _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync((User?)null);

        var result = await _service.UpdateUser(userId, updateUserDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUser_ThrowsException_WhenOldPasswordIsIncorrect()
    {
        var userId = 1;
        var user = new User("user1@example.com", "oldUsername", "password1", "Dark");
        var updateUserDto = new UpdateUserDto { OldPassword = "wrongPassword", NewPassword = "newPassword123" };

        _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
        _mockEncrypt.Setup(e => e.ComparePassword(updateUserDto.OldPassword, user.Password)).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateUser(userId, updateUserDto));
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk_WithElementDeleted()
    {
        var userId = 1;
        var user = new User("user1@example.com", "username", "password1", "Dark");
        var deletedUser = new User("user1@example.com", "username", "password1", "Dark");
        var outputUserDto = new OutputUserDto { Id = userId, Email = "user1@example.com", Username = "username", Theme = "Dark" };

        _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteUser(It.IsAny<User>())).ReturnsAsync(deletedUser);
        _mockMapper.Setup(m => m.Map<OutputUserDto>(deletedUser)).Returns(outputUserDto);

        var result = await _service.DeleteUser(userId);

        Assert.NotNull(result);
        Assert.Equal(outputUserDto.Id, result.Id);
        Assert.Equal(outputUserDto.Email, result.Email);
        Assert.Equal(outputUserDto.Username, result.Username);
        Assert.Equal(outputUserDto.Theme, result.Theme);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNull_WhenUserNotFound()
    {
        var userId = 1;

        _mockUserRepository.Setup(r => r.GetUserById(userId)).ReturnsAsync((User?)null);

        var result = await _service.DeleteUser(userId);

        Assert.Null(result);
    }

}