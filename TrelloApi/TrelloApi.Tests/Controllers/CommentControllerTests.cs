using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;

namespace TrelloApi.Tests.Controllers
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _mockCommentService;
        private readonly Mock<ILogger<CommentController>> _mockLogger;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _mockCommentService = new Mock<ICommentService>();
            _mockLogger = new Mock<ILogger<CommentController>>();

            _controller = new CommentController(_mockLogger.Object, _mockCommentService.Object);
            SetUserId(1);
        }
        
        private void SetUserId(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        [Fact]
        public async Task GetCommentById_ReturnsOk_WhenCommentFound()
        {
            int userId = 1;
            int commentId = 1;
            var outputComment = new OutputCommentDetailsDto 
            { 
                Id = commentId, 
                Text = "Comment 1", 
                Date = new DateTime(1, 1, 1), 
                CardId = 10, 
                AuthorId = 1 
            };

            _mockCommentService
                .Setup(s => s.GetCommentById(commentId, userId))
                .ReturnsAsync(outputComment);

            var result = await _controller.GetCommentById(commentId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedComment = Assert.IsType<OutputCommentDetailsDto>(okResult.Value);
            Assert.Equal(outputComment.Id, returnedComment.Id);
            Assert.Equal(outputComment.Text, returnedComment.Text);
            Assert.Equal(outputComment.Date, returnedComment.Date);
            Assert.Equal(outputComment.CardId, returnedComment.CardId);
            Assert.Equal(outputComment.AuthorId, returnedComment.AuthorId);
        }
        
        [Fact]
        public async Task GetCommentById_ReturnsNotFound_WhenCommentNotFound()
        {
            int userId = 1;
            int commentId = 1;
            OutputCommentDetailsDto? outputComment = null;
            
            _mockCommentService
                .Setup(s => s.GetCommentById(commentId, userId))
                .ReturnsAsync(outputComment);

            var result = await _controller.GetCommentById(commentId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetCommentsByCardId_ReturnsOk_WithComments()
        {
            int userId = 1;
            int cardId = 1;
            var comments = new List<OutputCommentDetailsDto>
            {
                new OutputCommentDetailsDto { Id = 1, Text = "Comment 1", Date = new DateTime(), CardId = cardId, AuthorId = 1 },
                new OutputCommentDetailsDto { Id = 2, Text = "Comment 2", Date = new DateTime(), CardId = cardId, AuthorId = 2 }
            };
            
            _mockCommentService
                .Setup(s => s.GetCommentsByCardId(cardId, userId))
                .ReturnsAsync(comments);

            var result = await _controller.GetCommentsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedComments = Assert.IsType<List<OutputCommentDetailsDto>>(okResult.Value);
            Assert.Equal(2, returnedComments.Count);
        }
        
        [Fact]
        public async Task GetCommentsByCardId_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            int cardId = 1;
            var comments = new List<OutputCommentDetailsDto>();
            
            _mockCommentService
                .Setup(s => s.GetCommentsByCardId(cardId, userId))
                .ReturnsAsync(comments);

            var result = await _controller.GetCommentsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedComments = Assert.IsType<List<OutputCommentDetailsDto>>(okResult.Value);
            Assert.Empty(returnedComments);
        }
        
        [Fact]
        public async Task AddComment_ReturnsCreated_WhenCommentIsAdded()
        {
            int userId = 1;
            int cardId = 1;
            var addCommentDto = new AddCommentDto 
            { 
                Text = "New Comment", 
                CardId = cardId, 
                AuthorId = 1 
            };
            var outputComment = new OutputCommentDetailsDto 
            { 
                Id = 2, 
                Text = "New Comment", 
                Date = new DateTime(), 
                CardId = cardId, 
                AuthorId = 1 
            };
            
            _mockCommentService
                .Setup(s => s.AddComment(cardId, addCommentDto, userId))
                .ReturnsAsync(outputComment);

            var result = await _controller.AddComment(cardId, addCommentDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnedComment = Assert.IsType<OutputCommentDetailsDto>(createdResult.Value);
            Assert.Equal(outputComment.Id, returnedComment.Id);
            Assert.Equal(outputComment.Text, returnedComment.Text);
            Assert.Equal(outputComment.Date, returnedComment.Date);
            Assert.Equal(outputComment.CardId, returnedComment.CardId);
            Assert.Equal(outputComment.AuthorId, returnedComment.AuthorId);
        }
        
        [Fact]
        public async Task AddComment_ReturnsBadRequest_WhenCommentIsNotAdded()
        {
            int userId = 1;
            int cardId = 1;
            var addCommentDto = new AddCommentDto 
            { 
                Text = "New Comment", 
                CardId = cardId, 
                AuthorId = 1 
            };
            
            _mockCommentService
                .Setup(s => s.AddComment(cardId, addCommentDto, userId))
                .ReturnsAsync((OutputCommentDetailsDto?)null);

            var result = await _controller.AddComment(cardId, addCommentDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateComment_ReturnsOk_WhenCommentIsUpdated()
        {
            int userId = 1;
            int commentId = 1;
            var updateCommentDto = new UpdateCommentDto { Text = "Updated Comment" };
            var outputComment = new OutputCommentDetailsDto 
            { 
                Id = commentId, 
                Text = "Updated Comment", 
                Date = new DateTime(), 
                CardId = 1, 
                AuthorId = 1 
            };
            
            _mockCommentService
                .Setup(s => s.UpdateComment(commentId, updateCommentDto, userId))
                .ReturnsAsync(outputComment);

            var result = await _controller.UpdateComment(commentId, updateCommentDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedComment = Assert.IsType<OutputCommentDetailsDto>(okResult.Value);
            Assert.Equal(outputComment.Id, returnedComment.Id);
            Assert.Equal(outputComment.Text, returnedComment.Text);
            Assert.Equal(outputComment.Date, returnedComment.Date);
            Assert.Equal(outputComment.CardId, returnedComment.CardId);
            Assert.Equal(outputComment.AuthorId, returnedComment.AuthorId);
        }
        
        [Fact]
        public async Task UpdateComment_ReturnsNotFound_WhenCommentNotFound()
        {
            int userId = 1;
            int commentId = 1;
            var updateCommentDto = new UpdateCommentDto { Text = "Updated Comment" };
            OutputCommentDetailsDto? outputComment = null;
            
            _mockCommentService
                .Setup(s => s.UpdateComment(commentId, updateCommentDto, userId))
                .ReturnsAsync(outputComment);

            var result = await _controller.UpdateComment(commentId, updateCommentDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteComment_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int userId = 1;
            int commentId = 1;
            
            _mockCommentService
                .Setup(s => s.DeleteComment(commentId, userId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteComment(commentId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteComment_ReturnsNotFound_WhenDeletionFails()
        {
            int userId = 1;
            int commentId = 1;
            
            _mockCommentService
                .Setup(s => s.DeleteComment(commentId, userId))
                .ReturnsAsync(false);

            var result = await _controller.DeleteComment(commentId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
