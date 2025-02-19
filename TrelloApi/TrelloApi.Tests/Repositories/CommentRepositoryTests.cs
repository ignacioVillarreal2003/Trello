using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.app;
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
    public async Task GetCommentById_ShouldReturnComment_WhenCommentExists()
    {
        int commentId = 1;
        var comment = new Comment(text: "text", cardId: 1, authorId: 1) { Id = commentId };
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCommentById(commentId);
        
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
    }

    [Fact]
    public async Task GetCommentById_ShouldReturnNull_WhenCommentDoesNotExist()
    {
        int commentId = 1;
        
        var result = await _repository.GetCommentById(commentId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnComments_WhenCardHasComments()
    {
        int cardId = 1;
        var comment1 = new Comment(text: "text 1", cardId: 1, authorId: 1) { Id = 1 };
        var comment2 = new Comment(text: "text 2", cardId: 1, authorId: 1) { Id = 2 };

        _context.Comments.AddRange(comment1, comment2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetCommentsByCardId(cardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnEmptyList_WhenCardHasNoComments()
    {
        int cardId = 1;
        
        var result = await _repository.GetCommentsByCardId(cardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddComment_ShouldPersistComment_WhenAddedSuccessfully()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1 ) { Id = 1 };
        
        await _repository.AddComment(comment);
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }

    [Fact]
    public async Task UpdateComment_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1 ) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        comment.Text = "updated text";
        await _repository.UpdateComment(comment);
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal(comment.Text, result.Text);
    }

    [Fact]
    public async Task DeleteComment_ShouldRemoveComment_WhenCommentExists()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1 ) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteComment(comment);
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.Null(result);
    }
}
