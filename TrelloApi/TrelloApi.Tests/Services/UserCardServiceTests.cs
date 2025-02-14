using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class UserCardServiceTests
{
    private readonly Mock<IUserCardRepository> _mockUserTaskRepository;
    private readonly Mock<ILogger<UserCardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly UserCardService _service;

    public UserCardServiceTests()
    {
        _mockUserTaskRepository = new Mock<IUserCardRepository>();
        _mockLogger = new Mock<ILogger<UserCardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new UserCardService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockUserTaskRepository.Object);
    }
    
    [Fact]
    public async Task GetUserTaskById_ReturnsOutputUserTaskDto_WhenUserTaskExists()
    {
        int taskId = 1, userId = 1;
        var userTask = new UserCard(userId: userId, taskId: taskId);
        var outputUserTaskDto = new OutputUserTaskDto { UserId = userTask.UserId, TaskId = userTask.TaskId };

        _mockUserTaskRepository.Setup(r => r.GetUserTaskById(userId, taskId)).ReturnsAsync(userTask);
        _mockMapper.Setup(m => m.Map<OutputUserTaskDto>(userTask)).Returns(outputUserTaskDto);

        var result = await _service.GetUserTaskById(userId, taskId);

        Assert.NotNull(result);
        Assert.Equal(outputUserTaskDto.UserId, result.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, result.TaskId);
    }

    [Fact]
    public async Task GetUserTaskById_ReturnsNull_WhenUserTaskDoesNotExist()
    {
        int taskId = 1, userId = 1;
        
        _mockUserTaskRepository.Setup(r => r.GetUserTaskById(userId, taskId)).ReturnsAsync((UserCard?)null);

        var result = await _service.GetUserTaskById(userId, taskId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task AddUserTask_ReturnsOutputUserTaskDto_WhenUserTaskIsAdded()
    {
        int taskId = 1, userId = 1;
        var addUserTaskDto = new AddUserTaskDto { UserId = userId, TaskId = taskId };
        var newUserTask = new UserCard(userId: addUserTaskDto.UserId, taskId: addUserTaskDto.TaskId);
        var outputUserTaskDto = new OutputUserTaskDto { UserId = newUserTask.UserId, TaskId = newUserTask.TaskId };

        _mockUserTaskRepository.Setup(r => r.AddUserTask(It.IsAny<UserCard>())).ReturnsAsync(newUserTask);
        _mockMapper.Setup(m => m.Map<OutputUserTaskDto>(newUserTask)).Returns(outputUserTaskDto);

        var result = await _service.AddUserTask(addUserTaskDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputUserTaskDto.UserId, result.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, result.TaskId);
    }

    [Fact]
    public async Task AddUserTask_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int taskId = 1, userId = 1;
        var addUserTaskDto = new AddUserTaskDto { UserId = userId, TaskId = taskId };

        _mockUserTaskRepository.Setup(r => r.AddUserTask(It.IsAny<UserCard>())).ReturnsAsync((UserCard?)null);

        var result = await _service.AddUserTask(addUserTaskDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteUserTask_ReturnsOutputUserTaskDto_WhenDeletionIsSuccessful()
    {
        int taskId = 1, userId = 1, userToDeleteId = 2;
        var existingUserTask = new UserCard(userId: userToDeleteId, taskId: taskId);
        var deletedUserTask = existingUserTask;
        var outputUserTaskDto = new OutputUserTaskDto { UserId = deletedUserTask.UserId, TaskId = deletedUserTask.TaskId };

        _mockUserTaskRepository.Setup(r => r.GetUserTaskById(userToDeleteId, taskId)).ReturnsAsync(existingUserTask);
        _mockUserTaskRepository.Setup(r => r.DeleteUserTask(existingUserTask)).ReturnsAsync(deletedUserTask);
        _mockMapper.Setup(m => m.Map<OutputUserTaskDto>(deletedUserTask)).Returns(outputUserTaskDto);

        var result = await _service.DeleteUserTask(userToDeleteId, taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputUserTaskDto.UserId, result.UserId);
        Assert.Equal(outputUserTaskDto.TaskId, result.TaskId);
    }

    [Fact]
    public async Task DeleteUserTask_ReturnsNull_WhenUserTaskNotFound()
    {
        int taskId = 1, userId = 1, userToDeleteId = 2;
        
        _mockUserTaskRepository.Setup(r => r.GetUserTaskById(userToDeleteId, taskId)).ReturnsAsync((UserCard?)null);

        var result = await _service.DeleteUserTask(userToDeleteId, userId, taskId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserTask_ReturnsNull_WhenDeletionFails()
    {
        int taskId = 1, userId = 1, userToDeleteId = 2;
        var existingUserTask = new UserCard(userId: userToDeleteId, taskId: taskId);

        _mockUserTaskRepository.Setup(r => r.GetUserTaskById(userToDeleteId, taskId)).ReturnsAsync(existingUserTask);
        _mockUserTaskRepository.Setup(r => r.DeleteUserTask(existingUserTask)).ReturnsAsync((UserCard?)null);

        var result = await _service.DeleteUserTask(userToDeleteId, taskId, userId);

        Assert.Null(result);
    }
}