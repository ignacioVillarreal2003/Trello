using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Comment;

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
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
    
            var httpContext = new DefaultHttpContext { User = principal };
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        [Fact]
        public async Task GetCommentById_ShouldReturnsOk_WhenCommentFound()
        {
            const int commentId = 1;
            var response = new CommentResponse 
            { 
                Id = commentId, 
                Text = "text", 
                CardId = 1, 
                AuthorId = 1 
            };

            _mockCommentService.Setup(s => s.GetCommentById(commentId)).ReturnsAsync(response);

            var result = await _controller.GetCommentById(commentId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task GetCommentById_ShouldReturnsNotFound_WhenCommentNotFound()
        {
            const int commentId = 1;
            
            _mockCommentService.Setup(s => s.GetCommentById(commentId)).ReturnsAsync((CommentResponse?)null);

            var result = await _controller.GetCommentById(commentId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetCommentsByCardId_ShouldReturnsOk_WithCommentsFound()
        {
            const int cardId = 1;
            var response = new List<CommentResponse>
            {
                new CommentResponse { Id = 1, Text = "text 1", CardId = cardId, AuthorId = 1 },
                new CommentResponse { Id = 2, Text = "text 2", CardId = cardId, AuthorId = 2 }
            };
            
            _mockCommentService.Setup(s => s.GetCommentsByCardId(cardId)).ReturnsAsync(response);

            var result = await _controller.GetCommentsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<CommentResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetCommentsByCardId_ShouldReturnsOk_WithCommentsNotFound()
        {
            const int cardId = 1;
            
            _mockCommentService.Setup(s => s.GetCommentsByCardId(cardId)).ReturnsAsync([]);

            var result = await _controller.GetCommentsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<CommentResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task AddComment_ShouldReturnsCreated_WhenCommentIsAdded()
        {
            const int userId = 1, cardId = 1;
            var dto = new AddCommentDto { Text = "text" };
            var response = new CommentResponse 
            { 
                Id = 1, 
                Text = dto.Text, 
                CardId = cardId, 
                AuthorId = 1 
            };
            
            _mockCommentService.Setup(s => s.AddComment(cardId, dto, userId)).ReturnsAsync(response);

            var result = await _controller.AddComment(cardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddComment_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int userId = 1, cardId = 1;
            var dto = new AddCommentDto { Text = "text", };
            
            _mockCommentService.Setup(s => s.AddComment(cardId, dto, userId)).ReturnsAsync((CommentResponse?)null);

            var result = await _controller.AddComment(cardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateComment_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int commentId = 1;
            var dto = new UpdateCommentDto { Text = "updated text" };
            var response = new CommentResponse 
            { 
                Id = commentId, 
                Text = dto.Text, 
                CardId = 1, 
                AuthorId = 1 
            };
            
            _mockCommentService.Setup(s => s.UpdateComment(commentId, dto)).ReturnsAsync(response);

            var result = await _controller.UpdateComment(commentId, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateComment_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int commentId = 1;
            var updateCommentDto = new UpdateCommentDto { Text = "updated text" };
            
            _mockCommentService.Setup(s => s.UpdateComment(commentId, updateCommentDto)).ReturnsAsync((CommentResponse?)null);

            var result = await _controller.UpdateComment(commentId, updateCommentDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteComment_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int commentId = 1;
            
            _mockCommentService
                .Setup(s => s.DeleteComment(commentId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteComment(commentId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteComment_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int commentId = 1;
            
            _mockCommentService.Setup(s => s.DeleteComment(commentId)).ReturnsAsync(false);

            var result = await _controller.DeleteComment(commentId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
