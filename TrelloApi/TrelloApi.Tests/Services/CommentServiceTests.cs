using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly Mock<ILogger<CommentService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockCommentRepository = new Mock<ICommentRepository>();
        _mockLogger = new Mock<ILogger<CommentService>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        
        _service = new CommentService(
            _mockMapper.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockCommentRepository.Object);
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsOutputCommentDetailsDto_WhenCommentExists()
    {
        const int commentId = 1;
        var comment = new Comment(text: "text 1", cardId: 1, authorId: 1) { Id = commentId };
        var response = new CommentResponse { Id = comment.Id, Text = comment.Text, CardId = comment.CardId, AuthorId = 1 };

        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        _mockMapper.Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>())).Returns(response);

        var result = await _service.GetCommentById(commentId);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCommentById_ShouldReturnsNull_WhenCommentNotFound()
    {
        const int commentId = 1;
        
        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync((Comment?)null);

        var result = await _service.GetCommentById(commentId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnsComments_WhenCommentsFound()
    {
        const int cardId = 1;
        var comments = new List<Comment>
        {
            new Comment(text: "text 1", cardId: cardId, authorId: 1) { Id = 1 },
            new Comment(text: "text 2", cardId: cardId, authorId: 2) { Id = 2 }
        };
        var response = new List<CommentResponse>
        {
            new CommentResponse { Id = comments[0].Id, Text = comments[0].Text, AuthorId = comments[0].AuthorId },
            new CommentResponse { Id = comments[1].Id, Text = comments[1].Text, AuthorId = comments[1].AuthorId }
        };

        _mockCommentRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Comment, bool>>>(), null)).ReturnsAsync(comments);
        _mockMapper.Setup(m => m.Map<List<CommentResponse>>(It.IsAny<List<Comment>>())).Returns(response);

        var result = await _service.GetCommentsByCardId(cardId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnsEmptyList_WhenCommentsNotFound()
    {
        const int cardId = 1;
        
        _mockCommentRepository.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<Comment, bool>>>(), null)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<CommentResponse>>(It.IsAny<List<Comment>>())).Returns([]);

        var result = await _service.GetCommentsByCardId(cardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task AddComment_ShouldReturnsComment_WhenAddedSuccessful()
    {
        const int cardId = 1, userId = 1;
        var dto = new AddCommentDto { Text = "text" };
        var response = new CommentResponse { Id = 1, Text = dto.Text, CardId = cardId, AuthorId = userId };

        _mockCommentRepository.Setup(r => r.CreateAsync(It.IsAny<Comment>()));
        _mockMapper.Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>())).Returns(response);

        var result = await _service.AddComment(cardId, dto, userId);

        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task UpdateComment_ShouldReturnsComment_WhenUpdatedSuccessful()
    {
        const int commentId = 1;
        var comment = new Comment(text: "text", cardId: 1, authorId: 1) { Id = commentId };
        var dto = new UpdateCommentDto { Text = "updated text" };
        var response = new CommentResponse { Id = comment.Id, Text = dto.Text, AuthorId = 1, CardId = comment.CardId };
        
        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        _mockCommentRepository.Setup(r => r.UpdateAsync(It.IsAny<Comment>()));
        _mockMapper.Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>())).Returns(response);

        var result = await _service.UpdateComment(commentId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateComment_ShouldReturnsNull_WhenUpdatedUnsuccessful()
    {
        const int commentId = 1;
        var dto = new UpdateCommentDto { Text = "updated text" };

        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync((Comment?)null);

        var result = await _service.UpdateComment(commentId, dto);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteComment_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int commentId = 1;
        var comment = new Comment(text: "text", cardId: 1, authorId: 1) { Id = commentId };

        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        _mockCommentRepository.Setup(r => r.DeleteAsync(It.IsAny<Comment>()));

        var result = await _service.DeleteComment(commentId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteComment_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int commentId = 1;
        
        _mockCommentRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync((Comment?)null);

        var result = await _service.DeleteComment(commentId);

        Assert.False(result);
    }
}
