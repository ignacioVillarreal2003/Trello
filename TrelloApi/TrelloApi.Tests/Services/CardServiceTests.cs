using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities.Card;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = TrelloApi.Domain.Entities.Card.Task;

namespace TrelloApi.Tests.Services;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _mockTaskRepository;
    private readonly Mock<ILogger<CardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly CardService _service;

    public CardServiceTests()
    {
        _mockTaskRepository = new Mock<ICardRepository>();
        _mockLogger = new Mock<ILogger<CardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new CardService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockTaskRepository.Object);
    }
    
    [Fact]
    public async Task GetTaskById_ReturnsOutputTaskDto_WhenTaskExists()
    {
        int taskId = 1, userId = 1;
        var task = new Task(title: "Task 1", description: "", listId: 1, priority: "Medium") { Id = taskId };
        var outputTaskDto = new OutputTaskDto { Id = task.Id, Title = task.Title, Priority = task.Priority, Description = task.Description, IsCompleted = task.IsCompleted};

        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync(task);
        _mockMapper.Setup(m => m.Map<OutputTaskDto>(task)).Returns(outputTaskDto);

        var result = await _service.GetTaskById(taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskDto.Id, result.Id);
        Assert.Equal(outputTaskDto.Title, result.Title);
        Assert.Equal(outputTaskDto.Priority, result.Priority);
        Assert.Equal(outputTaskDto.Description, result.Description);
        Assert.Equal(outputTaskDto.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task GetTaskById_ReturnsNull_WhenTaskDoesNotExist()
    {
        int taskId = 1, userId = 1;
        
        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync((Task?)null);

        var result = await _service.GetTaskById(taskId, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetTasksByListId_ReturnsListOfOutputTaskDto_WhenTasksExist()
    {
        int listId = 1, userId = 1;
        var tasks = new List<Task>
        {
            new Task(title: "Task 1", description: "", listId: listId, priority: "Medium") { Id = 1 },
            new Task(title: "Task 2", description: "", listId: listId, priority: "Medium") { Id = 2 }
        };
        var outputTaskDtos = new List<OutputTaskDto>
        {
            new OutputTaskDto { Id = tasks[0].Id, Title = tasks[0].Title, Priority = tasks[0].Priority, Description = tasks[0].Description, IsCompleted = tasks[0].IsCompleted },
            new OutputTaskDto { Id = tasks[1].Id, Title = tasks[1].Title, Priority = tasks[1].Priority, Description = tasks[1].Description, IsCompleted = tasks[1].IsCompleted }
        };

        _mockTaskRepository.Setup(r => r.GetTasksByListId(listId)).ReturnsAsync(tasks);
        _mockMapper.Setup(m => m.Map<List<OutputTaskDto>>(tasks)).Returns(outputTaskDtos);

        var result = await _service.GetTasksByListId(listId, userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(outputTaskDtos[0].Id, result[0].Id);
        Assert.Equal(outputTaskDtos[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetTasksByListId_ReturnsEmptyList_WhenNoTasksExist()
    {
        int listId = 1, userId = 1;

        _mockTaskRepository.Setup(r => r.GetTasksByListId(listId)).ReturnsAsync(new List<Task>());
        _mockMapper.Setup(m => m.Map<List<OutputTaskDto>>(It.IsAny<List<Task>>())).Returns(new List<OutputTaskDto>());

        var result = await _service.GetTasksByListId(listId, userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddTask_ReturnsOutputTaskDto_WhenTaskIsAdded()
    {
        int listId = 1, userId = 1;
        var addTaskDto = new AddTaskDto { Title = "New Task", Description = "" };
        var newTask = new Task(title: addTaskDto.Title, listId: listId, description: addTaskDto.Description) { Id = 1 };
        var outputTaskDto = new OutputTaskDto { Id = newTask.Id, Title = newTask.Title, Description = newTask.Description, Priority = newTask.Priority, IsCompleted = newTask.IsCompleted };

        _mockTaskRepository.Setup(r => r.AddTask(It.IsAny<Task>())).ReturnsAsync(newTask);
        _mockMapper.Setup(m => m.Map<OutputTaskDto>(newTask)).Returns(outputTaskDto);

        var result = await _service.AddTask(listId, addTaskDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskDto.Id, result.Id);
        Assert.Equal(outputTaskDto.Title, result.Title);
        Assert.Equal(outputTaskDto.Description, result.Description);
        Assert.Equal(outputTaskDto.Priority, result.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task AddTask_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int listId = 1, userId = 1;
        var addTaskDto = new AddTaskDto { Title = "New Task", Description = "" };

        _mockTaskRepository.Setup(r => r.AddTask(It.IsAny<Task>())).ReturnsAsync((Task?)null);

        var result = await _service.AddTask(listId, addTaskDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateTask_ReturnsOutputTaskDto_WhenUpdateIsSuccessful()
    {
        int taskId = 1, userId = 1;
        var existingTask = new Task(title: "Old Title", description: "", listId: 1) { Id = taskId };
        var updateTaskDto = new UpdateTaskDto { Title = "New Title" };
        var updatedTask = new Task(title: updateTaskDto.Title, listId: existingTask.ListId, description: existingTask.Description) { Id = existingTask.Id };
        var outputTaskDto = new OutputTaskDto { Id = updatedTask.Id, Title = updatedTask.Title, Description = updatedTask.Description, Priority = updatedTask.Priority, IsCompleted = updatedTask.IsCompleted };

        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.UpdateTask(existingTask)).ReturnsAsync(updatedTask);
        _mockMapper.Setup(m => m.Map<OutputTaskDto>(updatedTask)).Returns(outputTaskDto);

        var result = await _service.UpdateTask(taskId, updateTaskDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskDto.Id, result.Id);
        Assert.Equal(outputTaskDto.Title, result.Title);
        Assert.Equal(outputTaskDto.Description, result.Description);
        Assert.Equal(outputTaskDto.Priority, result.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNull_WhenTaskNotFound()
    {
        int taskId = 1, userId = 1;
        var updateTaskDto = new UpdateTaskDto { Title = "New Title" };

        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync((Task?)null);

        var result = await _service.UpdateTask(taskId, updateTaskDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNull_WhenUpdateFails()
    {
        int taskId = 1, userId = 1;
        var existingTask = new Task(title: "Old Title", description: "", listId: 1) { Id = taskId };
        var updateTaskDto = new UpdateTaskDto { Title = "New Title" };

        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.UpdateTask(existingTask)).ReturnsAsync((Task?)null);

        var result = await _service.UpdateTask(taskId, updateTaskDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteTask_ReturnsOutputTaskDto_WhenDeletionIsSuccessful()
    {
        int taskId = 1, userId = 1;
        var existingTask = new Task(title: "Old Title", description: "", listId: 1) { Id = taskId };
        var deletedTask = existingTask;
        var outputTaskDto = new OutputTaskDto { Id = deletedTask.Id, Title = deletedTask.Title, Description = deletedTask.Description, Priority = deletedTask.Priority, IsCompleted = deletedTask.IsCompleted };

        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.DeleteTask(existingTask)).ReturnsAsync(deletedTask);
        _mockMapper.Setup(m => m.Map<OutputTaskDto>(deletedTask)).Returns(outputTaskDto);

        var result = await _service.DeleteTask(taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskDto.Id, result.Id);
        Assert.Equal(outputTaskDto.Title, result.Title);
        Assert.Equal(outputTaskDto.Description, result.Description);
        Assert.Equal(outputTaskDto.Priority, result.Priority);
        Assert.Equal(outputTaskDto.IsCompleted, result.IsCompleted);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNull_WhenTaskNotFound()
    {
        int taskId = 1, userId = 1;
        
        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync((Task?)null);

        var result = await _service.DeleteTask(taskId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNull_WhenDeletionFails()
    {
        int taskId = 1, userId = 1;
        var existingTask = new Task(title: "Old Title", description: "", listId: 1) { Id = taskId };
        
        _mockTaskRepository.Setup(r => r.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.DeleteTask(existingTask)).ReturnsAsync((Task?)null);

        var result = await _service.DeleteTask(taskId, userId);

        Assert.Null(result);
    }
}
