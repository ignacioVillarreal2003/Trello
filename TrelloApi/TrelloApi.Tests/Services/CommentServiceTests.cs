using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly Mock<ILogger<CommentService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockCommentRepository = new Mock<ICommentRepository>();
        _mockLogger = new Mock<ILogger<CommentService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new CommentService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockCommentRepository.Object);
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsOutputCommentDto_WhenCommentExists()
    {
        int commentId = 1, userId = 1;
        var comment = new Comment(text: "Comment 1", taskId: 1, authorId: 1) { Id = commentId };
        var outputCommentDto = new OutputCommentDto { Id = comment.Id, Text = comment.Text, Date = comment.Date, AuthorUsername = ""};

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(comment);
        _mockMapper.Setup(m => m.Map<OutputCommentDto>(comment)).Returns(outputCommentDto);

        var result = await _service.GetCommentById(commentId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputCommentDto.Id, result.Id);
        Assert.Equal(outputCommentDto.Text, result.Text);
        Assert.Equal(outputCommentDto.Date, result.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, result.AuthorUsername);
    }

    [Fact]
    public async Task GetCommentById_ReturnsNull_WhenCommentDoesNotExist()
    {
        int commentId = 1, userId = 1;
        
        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync((Comment?)null);

        var result = await _service.GetCommentById(commentId, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetCommentsByTaskId_ReturnsListOfOutputCommentDto_WhenCommentsExist()
    {
        int taskId = 1, userId = 1;
        var comments = new List<Comment>
        {
            new Comment(text: "Comment 1", taskId: taskId, authorId: 1) { Id = 1 },
            new Comment(text: "Comment 2", taskId: taskId, authorId: 2) { Id = 2 }
        };
        var outputCommentDtos = new List<OutputCommentDto>
        {
            new OutputCommentDto { Id = comments[0].Id, Text = comments[0].Text, Date = comments[0].Date, AuthorUsername = "" },
            new OutputCommentDto { Id = comments[1].Id, Text = comments[1].Text, Date = comments[1].Date, AuthorUsername = "" }
        };

        _mockCommentRepository.Setup(r => r.GetCommentsByTaskId(taskId)).ReturnsAsync(comments);
        _mockMapper.Setup(m => m.Map<List<OutputCommentDto>>(comments)).Returns(outputCommentDtos);

        var result = await _service.GetCommentsByTaskId(taskId, userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(outputCommentDtos[0].Id, result[0].Id);
        Assert.Equal(outputCommentDtos[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetCommentsByTaskId_ReturnsEmptyComment_WhenNoCommentsExist()
    {
        int taskId = 1, userId = 1;

        _mockCommentRepository.Setup(r => r.GetCommentsByTaskId(taskId)).ReturnsAsync(new List<Comment>());
        _mockMapper.Setup(m => m.Map<List<OutputCommentDto>>(It.IsAny<List<Comment>>())).Returns(new List<OutputCommentDto>());

        var result = await _service.GetCommentsByTaskId(taskId, userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddComment_ReturnsOutputCommentDto_WhenCommentIsAdded()
    {
        int taskId = 1, userId = 1;
        var addCommentDto = new AddCommentDto { Text = "New Comment", AuthorId = userId };
        var newComment = new Comment(text: addCommentDto.Text, taskId: taskId, authorId: addCommentDto.AuthorId) { Id = 1 };
        var outputCommentDto = new OutputCommentDto { Id = newComment.Id, Text = newComment.Text, Date = newComment.Date, AuthorUsername = "" };

        _mockCommentRepository.Setup(r => r.AddComment(It.IsAny<Comment>())).ReturnsAsync(newComment);
        _mockMapper.Setup(m => m.Map<OutputCommentDto>(newComment)).Returns(outputCommentDto);

        var result = await _service.AddComment(taskId, addCommentDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputCommentDto.Id, result.Id);
        Assert.Equal(outputCommentDto.Text, result.Text);
        Assert.Equal(outputCommentDto.Date, result.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, result.AuthorUsername);
    }

    [Fact]
    public async Task AddComment_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int taskId = 1, userId = 1;
        var addCommentDto = new AddCommentDto { Text = "New Comment", AuthorId = userId };

        _mockCommentRepository.Setup(r => r.AddComment(It.IsAny<Comment>())).ReturnsAsync((Comment?)null);

        var result = await _service.AddComment(taskId, addCommentDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task UpdateComment_ReturnsOutputCommentDto_WhenUpdateIsSuccessful()
    {
        int commentId = 1, userId = 1;
        var existingComment = new Comment(text: "Old Text", taskId: 1, authorId: userId) { Id = 1 };
        var updateCommentDto = new UpdateCommentDto { Text = "New Text" };
        var updatedComment = new Comment(text: updateCommentDto.Text, taskId: existingComment.TaskId, authorId: existingComment.AuthorId) { Id = existingComment.Id };
        var outputCommentDto = new OutputCommentDto { Id = updatedComment.Id, Text = updatedComment.Text, Date = updatedComment.Date, AuthorUsername = "" };

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(existingComment);
        _mockCommentRepository.Setup(r => r.UpdateComment(existingComment)).ReturnsAsync(updatedComment);
        _mockMapper.Setup(m => m.Map<OutputCommentDto>(updatedComment)).Returns(outputCommentDto);

        var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputCommentDto.Id, result.Id);
        Assert.Equal(outputCommentDto.Text, result.Text);
        Assert.Equal(outputCommentDto.Date, result.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, result.AuthorUsername);
    }

    [Fact]
    public async Task UpdateComment_ReturnsNull_WhenCommentNotFound()
    {
        int commentId = 1, userId = 1;
        var updateCommentDto = new UpdateCommentDto { Text = "New Text" };

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync((Comment?)null);

        var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateComment_ReturnsNull_WhenUpdateFails()
    {
        int commentId = 1, userId = 1;
        var existingComment = new Comment(text: "Old Text", taskId: 1, authorId: userId) { Id = 1 };
        var updateCommentDto = new UpdateCommentDto { Text = "New Text" };

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(existingComment);
        _mockCommentRepository.Setup(r => r.UpdateComment(existingComment)).ReturnsAsync((Comment?)null);

        var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteComment_ReturnsOutputCommentDto_WhenDeletionIsSuccessful()
    {
        int commentId = 1, userId = 1;
        var existingComment = new Comment(text: "Old Text", taskId: 1, authorId: userId) { Id = 1 };
        var deletedComment = existingComment;
        var outputCommentDto = new OutputCommentDto { Id = deletedComment.Id, Text = deletedComment.Text, Date = deletedComment.Date, AuthorUsername = "" };

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(existingComment);
        _mockCommentRepository.Setup(r => r.DeleteComment(existingComment)).ReturnsAsync(deletedComment);
        _mockMapper.Setup(m => m.Map<OutputCommentDto>(deletedComment)).Returns(outputCommentDto);

        var result = await _service.DeleteComment(commentId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputCommentDto.Id, result.Id);
        Assert.Equal(outputCommentDto.Text, result.Text);
        Assert.Equal(outputCommentDto.Date, result.Date);
        Assert.Equal(outputCommentDto.AuthorUsername, result.AuthorUsername);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNull_WhenCommentNotFound()
    {
        int commentId = 1, userId = 1;
        
        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync((Comment?)null);

        var result = await _service.DeleteComment(commentId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteComment_ReturnsNull_WhenDeletionFails()
    {
        int commentId = 1, userId = 1;
        var existingComment = new Comment(text: "Old Text", taskId: 1, authorId: userId) { Id = 1 };

        _mockCommentRepository.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(existingComment);
        _mockCommentRepository.Setup(r => r.DeleteComment(existingComment)).ReturnsAsync((Comment?)null);

        var result = await _service.DeleteComment(commentId, userId);

        Assert.Null(result);
    }
}