using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.TaskLabel.Dto;

namespace TrelloApi.Tests.Services;

public class TaskLabelServiceTests
{
    private readonly Mock<ITaskLabelRepository> _mockTaskLabelRepository;
    private readonly Mock<ILogger<TaskLabelService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly TaskLabelService _service;

    public TaskLabelServiceTests()
    {
        _mockTaskLabelRepository = new Mock<ITaskLabelRepository>();
        _mockLogger = new Mock<ILogger<TaskLabelService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new TaskLabelService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockTaskLabelRepository.Object);
    }

    [Fact]
    public async Task GetTaskLabelById_ReturnsOutputTaskLabelDto_WhenTaskLabelExists()
    {
        int labelId = 1, taskId = 1, userId = 1;
        var taskLabel = new TaskLabel(taskId: taskId, labelId: labelId);
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = taskLabel.TaskId, LabelId = taskLabel.LabelId };

        _mockTaskLabelRepository.Setup(r => r.GetTaskLabelById(taskId, labelId)).ReturnsAsync(taskLabel);
        _mockMapper.Setup(m => m.Map<OutputTaskLabelDto>(taskLabel)).Returns(outputTaskLabelDto);

        var result = await _service.GetTaskLabelById(taskId, labelId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskLabelDto.TaskId, result.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, result.LabelId);
    }

    [Fact]
    public async Task GetTaskLabelById_ReturnsNull_WhenTaskLabelDoesNotExist()
    {
        int labelId = 1, taskId = 1, userId = 1;

        _mockTaskLabelRepository.Setup(r => r.GetTaskLabelById(taskId, labelId)).ReturnsAsync((TaskLabel?)null);

        var result = await _service.GetTaskLabelById(taskId, labelId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddTaskLabel_ReturnsOutputTaskLabelDto_WhenTaskLabelIsAdded()
    {
        int labelId = 1, taskId = 1, userId = 1;
        var addTaskLabelDto = new AddTaskLabelDto { TaskId = taskId, LabelId = labelId };
        var newTaskLabel = new TaskLabel(taskId: addTaskLabelDto.TaskId, labelId: addTaskLabelDto.LabelId);
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = newTaskLabel.TaskId, LabelId = newTaskLabel.LabelId };

        _mockTaskLabelRepository.Setup(r => r.AddTaskLabel(It.IsAny<TaskLabel>())).ReturnsAsync(newTaskLabel);
        _mockMapper.Setup(m => m.Map<OutputTaskLabelDto>(newTaskLabel)).Returns(outputTaskLabelDto);

        var result = await _service.AddTaskLabel(addTaskLabelDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskLabelDto.TaskId, result.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, result.LabelId);
    }

    [Fact]
    public async Task AddTaskLabel_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int labelId = 1, taskId = 1, userId = 1;
        var addTaskLabelDto = new AddTaskLabelDto { TaskId = taskId, LabelId = labelId };

        _mockTaskLabelRepository.Setup(r => r.AddTaskLabel(It.IsAny<TaskLabel>())).ReturnsAsync((TaskLabel?)null);

        var result = await _service.AddTaskLabel(addTaskLabelDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskLabel_ReturnsOutputTaskLabelDto_WhenDeletionIsSuccessful()
    {
        int labelId = 1, taskId = 1, userId = 1;
        var existingTaskLabel = new TaskLabel(taskId: taskId, labelId: labelId);
        var deletedTaskLabel = existingTaskLabel;
        var outputTaskLabelDto = new OutputTaskLabelDto { TaskId = deletedTaskLabel.TaskId, LabelId = deletedTaskLabel.LabelId };

        _mockTaskLabelRepository.Setup(r => r.GetTaskLabelById(taskId, labelId)).ReturnsAsync(existingTaskLabel);
        _mockTaskLabelRepository.Setup(r => r.DeleteTaskLabel(existingTaskLabel)).ReturnsAsync(deletedTaskLabel);
        _mockMapper.Setup(m => m.Map<OutputTaskLabelDto>(deletedTaskLabel)).Returns(outputTaskLabelDto);

        var result = await _service.DeleteTaskLabel(taskId, labelId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputTaskLabelDto.TaskId, result.TaskId);
        Assert.Equal(outputTaskLabelDto.LabelId, result.LabelId);
    }

    [Fact]
    public async Task DeleteTaskLabel_ReturnsNull_WhenTaskLabelNotFound()
    {
        int labelId = 1, taskId = 1, userId = 1;

        _mockTaskLabelRepository.Setup(r => r.GetTaskLabelById(taskId, labelId)).ReturnsAsync((TaskLabel?)null);

        var result = await _service.DeleteTaskLabel(taskId, labelId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskLabel_ReturnsNull_WhenDeletionFails()
    {
        int labelId = 1, taskId = 1, userId = 1;
        var existingTaskLabel = new TaskLabel(taskId: taskId, labelId: labelId);

        _mockTaskLabelRepository.Setup(r => r.GetTaskLabelById(taskId, labelId)).ReturnsAsync(existingTaskLabel);
        _mockTaskLabelRepository.Setup(r => r.DeleteTaskLabel(existingTaskLabel)).ReturnsAsync((TaskLabel?)null);

        var result = await _service.DeleteTaskLabel(taskId, labelId, userId);

        Assert.Null(result);
    }
}