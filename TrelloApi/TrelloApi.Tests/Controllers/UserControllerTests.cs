using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;

namespace TrelloApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtService> _mockJwt;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwt = new Mock<IJwtService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockLogger.Object, _mockUserService.Object, _mockJwt.Object);
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
        public async Task GetUsers_ShouldReturnsOk_WhenUsersFound()
        {
            var response = new List<UserResponse>
            {
                new UserResponse { Id = 1, Email = "user1@example.com", Username = "username 1", Theme = "theme" },
                new UserResponse { Id = 2, Email = "user2@example.com", Username = "username 2", Theme = "theme" }
            };

            _mockUserService.Setup(s => s.GetUsers()).ReturnsAsync(response);

            var result = await _controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(2, value.Count);
        }
        
        [Fact]
        public async Task GetUsers_ShouldReturnsOk_WhenUsersNotFound()
        {
            _mockUserService.Setup(s => s.GetUsers()).ReturnsAsync([]);

            var result = await _controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task GetUsersByUsername_ShouldReturnsOk_WhenUsersFound()
        {
            const string username = "user";
            var response = new List<UserResponse>
            {
                new UserResponse { Id = 1, Email = "user1@example.com", Username = "username 1", Theme = "theme" },
                new UserResponse { Id = 2, Email = "user2@example.com", Username = "username 2", Theme = "theme" }
            };

            _mockUserService.Setup(s => s.GetUsersByUsername(username)).ReturnsAsync(response);

            var result = await _controller.GetUsersByUsername(username);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(2, value.Count);
        }
        
        [Fact]
        public async Task GetUsersByUsername_ShouldReturnsOk_WhenUsersNotFound()
        {
            const string username = "user";

            _mockUserService.Setup(s => s.GetUsersByUsername(username)).ReturnsAsync([]);

            var result = await _controller.GetUsersByUsername(username);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task GetUsersByTaskId_ShouldReturnsOk_WhenUsersFound()
        {
            const int cardId = 1;
            var response = new List<UserResponse>
            {
                new UserResponse { Id = 1, Email = "user1@example.com", Username = "username 1", Theme = "theme" },
                new UserResponse { Id = 2, Email = "user2@example.com", Username = "username 2", Theme = "theme" }
            };

            _mockUserService.Setup(s => s.GetUsersByCardId(cardId)).ReturnsAsync(response);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(2, value.Count);
        }
        
        [Fact]
        public async Task GetUsersByTaskId_ShouldReturnsOk_WhenUsersNotFound()
        {
            const int cardId = 1;

            _mockUserService.Setup(s => s.GetUsersByCardId(cardId)).ReturnsAsync([]);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<UserResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task RegisterUser_ShouldReturnsCreated_WhenRegisteredSuccessful()
        {
            var dto = new RegisterUserDto { Email = "user@example.com", Username = "user", Password = "password" };
            var response = new UserResponse { Id = 1, Email = dto.Email, Username = dto.Username, Theme = "theme" };
            var token = "fake-jwt-token";

            _mockUserService.Setup(s => s.RegisterUser(dto)).ReturnsAsync(response);
            _mockJwt.Setup(j => j.GenerateAccessToken(response.Id)).Returns(token);
            _mockJwt.Setup(j => j.GenerateRefreshToken()).Returns(token);
            _mockJwt.Setup(j => j.SaveRefreshToken(response.Id, token)).Returns(Task.CompletedTask);
            
            var result = await _controller.RegisterUser(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task RegisterUser_ShouldReturnsBadRequest_WhenRegisteredUnsuccessful()
        {
            var dto = new RegisterUserDto { Email = "user@example.com", Username = "username", Password = "password" };

            _mockUserService.Setup(s => s.RegisterUser(dto)).ReturnsAsync((UserResponse?)null);

            var result = await _controller.RegisterUser(dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task LoginUser_ShouldReturnsOk_WhenLoggedSuccessful()
        {
            var dto = new LoginUserDto { Email = "user@example.com", Password = "password" };
            var response = new UserResponse { Id = 1, Email = dto.Email, Username = "username", Theme = "theme" };
            var token = "fake-jwt-token";

            _mockUserService.Setup(s => s.LoginUser(dto)).ReturnsAsync(response);
            _mockJwt.Setup(j => j.GenerateAccessToken(response.Id)).Returns(token);
            _mockJwt.Setup(j => j.GenerateRefreshToken()).Returns(token);
            _mockJwt.Setup(j => j.SaveRefreshToken(response.Id, token)).Returns(Task.CompletedTask);

            var result = await _controller.LoginUser(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task LoginUser_ShouldReturnsBadRequest_WhenLoggedUnsuccessful()
        {
            var dto = new LoginUserDto { Email = "user@example.com", Password = "password" };

            _mockUserService.Setup(s => s.LoginUser(dto)).ReturnsAsync((UserResponse?)null);
            
            var result = await _controller.LoginUser(dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateUser_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int userId = 1;
            var dto = new UpdateUserDto { Username = "updated username" };
            var response = new UserResponse
            {
                Id = userId, 
                Email = "user@example.com", 
                Username = dto.Username, 
                Theme = "theme"
            };
            _mockUserService.Setup(s => s.UpdateUser(dto, userId)).ReturnsAsync(response);

            var result = await _controller.UpdateUser(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateUser_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int userId = 1;
            var dto = new UpdateUserDto { Username = "updated username" };

            _mockUserService.Setup(s => s.UpdateUser(dto, userId)).ReturnsAsync((UserResponse?)null);

            var result = await _controller.UpdateUser(dto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteUser_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int userId = 1;

            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(true);

            var result = await _controller.DeleteUser();
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteUser_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int userId = 1; 
            
            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(false);

            var result = await _controller.DeleteUser();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
