using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;

namespace TrelloApi.Tests.Controllers
{
    public class UserBoardControllerTests
    {
        private readonly Mock<IUserBoardService> _mockUserBoardService;
        private readonly Mock<ILogger<UserBoardController>> _mockLogger;
        private readonly UserBoardController _controller;

        public UserBoardControllerTests()
        {
            _mockUserBoardService = new Mock<IUserBoardService>();
            _mockLogger = new Mock<ILogger<UserBoardController>>();
            _controller = new UserBoardController(_mockLogger.Object, _mockUserBoardService.Object);
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
        public async Task GetUsersByBoardId_ShouldReturnsOk_WithUsersFound()
        {
            const int boardId = 1;
            var response = new List<UserResponse>
            {
                new UserResponse { Id = 1, Email = "email1@test.com", Username = "username 1", Theme = "theme" },
                new UserResponse { Id = 2, Email = "email1@test.com", Username = "username 2", Theme = "theme" }
            };

            _mockUserBoardService.Setup(s => s.GetUsersByBoardId(boardId)).ReturnsAsync(response);

            var result = await _controller.GetUsersByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetUsersByBoardId_ShouldReturnsOk_WithUsersNotFound()
        {
            const int boardId = 1;

            _mockUserBoardService.Setup(s => s.GetUsersByBoardId(boardId)).ReturnsAsync([]);

            var result = await _controller.GetUsersByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task AddUserToBoard_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int boardId = 1;
            var dto = new AddUserBoardDto { UserId = 1, Role = "role" };
            var response = new UserBoardResponse { UserId = dto.UserId, BoardId = boardId, Role = dto.Role };

            _mockUserBoardService.Setup(s => s.AddUserToBoard(boardId, dto)).ReturnsAsync(response);

            var result = await _controller.AddUserToBoard(boardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddUserToBoard_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int boardId = 1;
            var dto = new AddUserBoardDto { UserId = 1, Role = "role" };

            _mockUserBoardService.Setup(s => s.AddUserToBoard(boardId, dto)).ReturnsAsync((UserBoardResponse?)null);

            var result = await _controller.AddUserToBoard(boardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromBoard_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int boardId = 1, userId = 2;

            _mockUserBoardService.Setup(s => s.RemoveUserFromBoard(boardId, userId)).ReturnsAsync(true);

            var result = await _controller.RemoveUserFromBoard(boardId, userId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromBoard_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int boardId = 1, userId = 2;

            _mockUserBoardService.Setup(s => s.RemoveUserFromBoard(boardId, userId)).ReturnsAsync(false);

            var result = await _controller.RemoveUserFromBoard(boardId, userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
