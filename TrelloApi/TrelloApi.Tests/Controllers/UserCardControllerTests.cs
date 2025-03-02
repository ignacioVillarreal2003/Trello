using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;

namespace TrelloApi.Tests.Controllers
{
    public class UserCardControllerTests
    {
        private readonly Mock<IUserCardService> _mockUserCardService;
        private readonly Mock<ILogger<UserCardController>> _mockLogger;
        private readonly UserCardController _controller;

        public UserCardControllerTests()
        {
            _mockUserCardService = new Mock<IUserCardService>();
            _mockLogger = new Mock<ILogger<UserCardController>>();
            _controller = new UserCardController(_mockLogger.Object, _mockUserCardService.Object);
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
        public async Task GetUsersByCardId_ShouldReturnsOk_WhenUsersFound()
        {
            const int cardId = 1;
            var response = new List<UserResponse>
            {
                new UserResponse { Id = 1, Email = "email1@test.com", Username = "username 1", Theme = "theme" },
                new UserResponse { Id = 2, Email = "email2@test.com", Username = "username 2", Theme = "theme" }
            };

            _mockUserCardService.Setup(s => s.GetUsersByCardId(cardId)).ReturnsAsync(response);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetUsersByCardId_ShouldReturnsOk_WhenUserNotFound()
        {
            const int cardId = 1;

            _mockUserCardService.Setup(s => s.GetUsersByCardId(cardId)).ReturnsAsync([]);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task AddUserToCard_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int cardId = 1;
            var dto = new AddUserCardDto { UserId = 1 };
            var response = new UserCardResponse { UserId = dto.UserId, CardId = cardId };

            _mockUserCardService.Setup(s => s.AddUserToCard(cardId, dto)).ReturnsAsync(response);

            var result = await _controller.AddUserToCard(cardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddUserToCard_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int cardId = 1;
            var dto = new AddUserCardDto { UserId = 1 };

            _mockUserCardService.Setup(s => s.AddUserToCard(cardId, dto)).ReturnsAsync((UserCardResponse?)null);

            var result = await _controller.AddUserToCard(cardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromCard_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int userId = 1, cardId = 1;

            _mockUserCardService.Setup(s => s.RemoveUserFromCard(userId, cardId)).ReturnsAsync(true);

            var result = await _controller.RemoveUserFromCard(userId, cardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromCard_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int userId = 1, cardId = 1;

            _mockUserCardService.Setup(s => s.RemoveUserFromCard(userId, cardId)).ReturnsAsync(false);

            var result = await _controller.RemoveUserFromCard(userId, cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
