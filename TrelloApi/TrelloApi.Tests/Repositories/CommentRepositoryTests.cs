using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Repositories;

public class CommentRepositoryTests
{
    private readonly CommentRepository _repository;
    private readonly TrelloContext _context;
    private readonly Mock<ILogger<CommentRepository>> _mockLogger;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _mockLogger = new Mock<ILogger<CommentRepository>>();
        _repository = new CommentRepository(_context, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsComment_WhenCommentExists()
    {
        int commentId = 1;
        var comment = new Comment(text: "Test Comment", taskId: 1, authorId: 1) { Id = commentId };
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCommentById(commentId);
        
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
    }

    [Fact]
    public async Task GetCommentById_ReturnsNull_WhenCommentDoesNotExist()
    {
        int commentId = 1;
        
        var result = await _repository.GetCommentById(commentId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCommentsByTaskId_ReturnsComments_WhenCommentsExistForUser()
    {
        int taskId = 1;
        var comment1 = new Comment(text: "Comment 1", taskId: 1, authorId: 1) { Id = 1 };
        var comment2 = new Comment(text: "Comment 2", taskId: 1, authorId: 1) { Id = 2 };

        _context.Comments.AddRange(comment1, comment2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCommentsByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCommentsByTaskId_ReturnsEmptyList_WhenNoCommentsExistForUser()
    {
        int taskId = 1;
        
        var result = await _repository.GetCommentsByTaskId(taskId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddComment_ReturnsComment_WhenCommentIsAddedSuccessfully()
    {
        var comment = new Comment(text: "New Comment", taskId: 1, authorId: 1 ) { Id = 1 };
        
        _context.Comments.RemoveRange(_context.Comments);
        await _context.SaveChangesAsync();
        
        var result = await _repository.AddComment(comment);
        
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }

    [Fact]
    public async Task UpdateComment_ReturnsComment_WhenCommentIsUpdatedSuccessfully()
    {
        var comment = new Comment(text: "Existing Comment", taskId: 1, authorId: 1 ) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        comment.Text = "Updated Comment";
        
        var result = await _repository.UpdateComment(comment);
        
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }

    [Fact]
    public async Task DeleteComment_ReturnsComment_WhenCommentIsDeletedSuccessfully()
    {
        var comment = new Comment(text: "Comment To Delete", taskId: 1, authorId: 1 ) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        var result = await _repository.DeleteComment(comment);
        
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }
}
