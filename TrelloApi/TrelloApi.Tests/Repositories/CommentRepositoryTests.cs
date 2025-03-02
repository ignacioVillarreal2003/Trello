using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using TrelloApi.Infrastructure.Persistence.Repositories;

namespace TrelloApi.Tests.Repositories;

public class CommentRepositoryTests
{
    private readonly ICommentRepository _repository;
    private readonly TrelloContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TrelloContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TrelloContext(options);
        _unitOfWork = new FakeUnitOfWork(_context);
        _repository = new CommentRepository(_unitOfWork);
    }
    
    [Fact]
    public async Task GetCommentById_ShouldReturnComment_WhenCommentExists()
    {
        int commentId = 1;
        var comment = new Comment("text", cardId: 1, authorId: 1) { Id = commentId };
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetAsync(c => c.Id == commentId);
        
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
    }

    [Fact]
    public async Task GetCommentById_ShouldReturnNull_WhenCommentDoesNotExist()
    {
        int commentId = 1;
        
        var result = await _repository.GetAsync(c => c.Id == commentId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnComments_WhenCardHasComments()
    {
        int cardId = 1;
        var comment1 = new Comment("text 1", cardId, authorId: 1) { Id = 1 };
        var comment2 = new Comment("text 2", cardId, authorId: 1) { Id = 2 };

        _context.Comments.AddRange(comment1, comment2);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetListAsync(c => c.CardId == cardId);
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnEmptyList_WhenCardHasNoComments()
    {
        int cardId = 1;
        
        var result = await _repository.GetListAsync(c => c.CardId == cardId);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddComment_ShouldPersistComment_WhenAddedSuccessfully()
    {
        var comment = new Comment("text", cardId: 1, authorId: 1) { Id = 1 };
        
        await _repository.CreateAsync(comment);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
    }

    [Fact]
    public async Task UpdateComment_ShouldPersistChanges_WhenUpdateIsSuccessful()
    {
        var comment = new Comment("text", cardId: 1, authorId: 1) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        comment.Text = "updated text";
        await _repository.UpdateAsync(comment);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.NotNull(result);
        Assert.Equal("updated text", result.Text);
    }

    [Fact]
    public async Task DeleteComment_ShouldRemoveComment_WhenCommentExists()
    {
        var comment = new Comment("text", cardId: 1, authorId: 1) { Id = 1 };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        await _repository.DeleteAsync(comment);
        await _unitOfWork.CommitAsync();
        
        var result = await _context.Comments.FindAsync(comment.Id);

        Assert.Null(result);
    }
}
