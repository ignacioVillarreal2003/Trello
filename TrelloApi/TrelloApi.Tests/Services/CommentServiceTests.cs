using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Services
{
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

            _service = new CommentService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockLogger.Object,
                _mockCommentRepository.Object);
        }
        
        [Fact]
        public async Task GetCommentById_ReturnsOutputCommentDetailsDto_WhenCommentExists()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var comment = new Comment(text: "Comment 1", cardId: 1, authorId: 1) { Id = commentId };
            var outputCommentDetailsDto = new OutputCommentDetailsDto 
            { 
                Id = comment.Id, 
                Text = comment.Text, 
                Date = comment.Date, 
                AuthorId = 1
            };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync(comment);
            _mockMapper
                .Setup(m => m.Map<OutputCommentDetailsDto>(comment))
                .Returns(outputCommentDetailsDto);

            // Act
            var result = await _service.GetCommentById(commentId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputCommentDetailsDto.Id, result.Id);
            Assert.Equal(outputCommentDetailsDto.Text, result.Text);
            Assert.Equal(outputCommentDetailsDto.Date, result.Date);
            Assert.Equal(outputCommentDetailsDto.AuthorId, result.AuthorId);
        }

        [Fact]
        public async Task GetCommentById_ReturnsNull_WhenCommentDoesNotExist()
        {
            // Arrange
            int commentId = 1, userId = 1;
            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.GetCommentById(commentId, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetCommentsByCardId_ReturnsListOfOutputCommentDetailsDto_WhenCommentsExist()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var comments = new List<Comment>
            {
                new Comment(text: "Comment 1", cardId: cardId, authorId: 1) { Id = 1 },
                new Comment(text: "Comment 2", cardId: cardId, authorId: 2) { Id = 2 }
            };
            var outputCommentDetailsDtos = new List<OutputCommentDetailsDto>
            {
                new OutputCommentDetailsDto { Id = comments[0].Id, Text = comments[0].Text, Date = comments[0].Date, AuthorId = 1 },
                new OutputCommentDetailsDto { Id = comments[1].Id, Text = comments[1].Text, Date = comments[1].Date, AuthorId = 1 }
            };

            _mockCommentRepository
                .Setup(r => r.GetCommentsByCardId(cardId))
                .ReturnsAsync(comments);
            _mockMapper
                .Setup(m => m.Map<List<OutputCommentDetailsDto>>(comments))
                .Returns(outputCommentDetailsDtos);

            // Act
            var result = await _service.GetCommentsByCardId(cardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(outputCommentDetailsDtos[0].Id, result[0].Id);
            Assert.Equal(outputCommentDetailsDtos[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetCommentsByCardId_ReturnsEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            int cardId = 1, userId = 1;
            _mockCommentRepository
                .Setup(r => r.GetCommentsByCardId(cardId))
                .ReturnsAsync(new List<Comment>());
            _mockMapper
                .Setup(m => m.Map<List<OutputCommentDetailsDto>>(It.IsAny<List<Comment>>()))
                .Returns(new List<OutputCommentDetailsDto>());

            // Act
            var result = await _service.GetCommentsByCardId(cardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task AddComment_ReturnsOutputCommentDetailsDto_WhenCommentIsAdded()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var addCommentDto = new AddCommentDto { Text = "New Comment", AuthorId = userId };
            var newComment = new Comment(text: addCommentDto.Text, cardId: cardId, authorId: addCommentDto.AuthorId) { Id = 1 };
            var outputCommentDetailsDto = new OutputCommentDetailsDto 
            { 
                Id = newComment.Id, 
                Text = newComment.Text, 
                Date = newComment.Date, 
                AuthorId = 1
            };

            _mockCommentRepository
                .Setup(r => r.AddComment(It.IsAny<Comment>()))
                .ReturnsAsync(newComment);
            _mockMapper
                .Setup(m => m.Map<OutputCommentDetailsDto>(newComment))
                .Returns(outputCommentDetailsDto);

            // Act
            var result = await _service.AddComment(cardId, addCommentDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputCommentDetailsDto.Id, result.Id);
            Assert.Equal(outputCommentDetailsDto.Text, result.Text);
            Assert.Equal(outputCommentDetailsDto.Date, result.Date);
            Assert.Equal(outputCommentDetailsDto.AuthorId, result.AuthorId);
        }

        [Fact]
        public async Task AddComment_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var addCommentDto = new AddCommentDto { Text = "New Comment", AuthorId = userId };

            _mockCommentRepository
                .Setup(r => r.AddComment(It.IsAny<Comment>()))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.AddComment(cardId, addCommentDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task UpdateComment_ReturnsOutputCommentDetailsDto_WhenUpdateIsSuccessful()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var existingComment = new Comment(text: "Old Text", cardId: 1, authorId: userId) { Id = commentId };
            var updateCommentDto = new UpdateCommentDto { Text = "New Text" };
            var updatedComment = new Comment(text: updateCommentDto.Text, cardId: existingComment.CardId, authorId: existingComment.AuthorId) { Id = existingComment.Id };
            var outputCommentDetailsDto = new OutputCommentDetailsDto 
            { 
                Id = updatedComment.Id, 
                Text = updatedComment.Text, 
                Date = updatedComment.Date, 
                AuthorId = 1 
            };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync(existingComment);
            _mockCommentRepository
                .Setup(r => r.UpdateComment(existingComment))
                .ReturnsAsync(updatedComment);
            _mockMapper
                .Setup(m => m.Map<OutputCommentDetailsDto>(updatedComment))
                .Returns(outputCommentDetailsDto);

            // Act
            var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputCommentDetailsDto.Id, result.Id);
            Assert.Equal(outputCommentDetailsDto.Text, result.Text);
            Assert.Equal(outputCommentDetailsDto.Date, result.Date);
            Assert.Equal(outputCommentDetailsDto.AuthorId, result.AuthorId);
        }

        [Fact]
        public async Task UpdateComment_ReturnsNull_WhenCommentNotFound()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var updateCommentDto = new UpdateCommentDto { Text = "New Text" };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateComment_ReturnsNull_WhenUpdateFails()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var existingComment = new Comment(text: "Old Text", cardId: 1, authorId: userId) { Id = commentId };
            var updateCommentDto = new UpdateCommentDto { Text = "New Text" };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync(existingComment);
            _mockCommentRepository
                .Setup(r => r.UpdateComment(existingComment))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.UpdateComment(commentId, updateCommentDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task DeleteComment_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var existingComment = new Comment(text: "Old Text", cardId: 1, authorId: userId) { Id = commentId };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync(existingComment);
            _mockCommentRepository
                .Setup(r => r.DeleteComment(existingComment))
                .ReturnsAsync(existingComment);

            // Act
            var result = await _service.DeleteComment(commentId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteComment_ReturnsFalse_WhenCommentNotFound()
        {
            // Arrange
            int commentId = 1, userId = 1;
            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.DeleteComment(commentId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteComment_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int commentId = 1, userId = 1;
            var existingComment = new Comment(text: "Old Text", cardId: 1, authorId: userId) { Id = commentId };

            _mockCommentRepository
                .Setup(r => r.GetCommentById(commentId))
                .ReturnsAsync(existingComment);
            _mockCommentRepository
                .Setup(r => r.DeleteComment(existingComment))
                .ReturnsAsync((Comment?)null);

            // Act
            var result = await _service.DeleteComment(commentId, userId);

            // Assert
            Assert.False(result);
        }
    }
}
