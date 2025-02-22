using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.DTOs;
using Xunit;

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
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        // GET /User - GetUsers
        [Fact]
        public async Task GetUsers_ReturnsOk_WithFullList()
        {
            int userId = 1;
            var listOutputUserDetailsDto = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
            };

            _mockUserService.Setup(s => s.GetUsers(userId)).ReturnsAsync(listOutputUserDetailsDto);

            var result = await _controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }
        
        [Fact]
        public async Task GetUsers_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            var listOutputUserDetailsDto = new List<OutputUserDetailsDto>();

            _mockUserService.Setup(s => s.GetUsers(userId)).ReturnsAsync(listOutputUserDetailsDto);

            var result = await _controller.GetUsers();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Empty(returnedList);
        }
        
        // GET /User/username/{username}
        [Fact]
        public async Task GetUsersByUsername_ReturnsOk_WithFullList()
        {
            int userId = 1;
            string username = "user";
            var listOutputUserDetailsDto = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
            };

            _mockUserService.Setup(s => s.GetUsersByUsername(username, userId)).ReturnsAsync(listOutputUserDetailsDto);

            var result = await _controller.GetUsersByUsername(username);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }
        
        [Fact]
        public async Task GetUsersByUsername_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            string username = "nouser";
            var listOutputUserDetailsDto = new List<OutputUserDetailsDto>();

            _mockUserService.Setup(s => s.GetUsersByUsername(username, userId)).ReturnsAsync(listOutputUserDetailsDto);

            var result = await _controller.GetUsersByUsername(username);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Empty(returnedList);
        }
        
        // GET /User/card/{cardId}
        [Fact]
        public async Task GetUsersByTaskId_ReturnsOk_WithFullList()
        {
            int userId = 1;
            int cardId = 1;
            var listOutputUsers = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" },
                new OutputUserDetailsDto { Id = 2, Email = "user2@example.com", Username = "user2", Theme = "Light" }
            };

            _mockUserService.Setup(s => s.GetUsersByCardId(cardId, userId)).ReturnsAsync(listOutputUsers);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }
        
        [Fact]
        public async Task GetUsersByTaskId_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            int cardId = 1;
            var listOutputUsers = new List<OutputUserDetailsDto>();

            _mockUserService.Setup(s => s.GetUsersByCardId(cardId, userId)).ReturnsAsync(listOutputUsers);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Empty(returnedList);
        }
        
        // POST /User/register-user
        [Fact]
        public async Task RegisterUser_ReturnsCreated_WithElementCreated()
        {
            var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
            var outputUserDetailsDto = new OutputUserDetailsDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" };
            var token = "fake-jwt-token";

            _mockUserService.Setup(s => s.RegisterUser(registerUserDto)).ReturnsAsync(outputUserDetailsDto);
            _mockJwt.Setup(j => j.GenerateToken(outputUserDetailsDto.Id)).Returns(token);

            var result = await _controller.RegisterUser(registerUserDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = createdResult.Value;
            Assert.NotNull(value);

            // Se extraen las propiedades "token" y "user" de la respuesta anónima
            var tokenValue = value.GetType().GetProperty("token")?.GetValue(value);
            var user = value.GetType().GetProperty("user")?.GetValue(value) as OutputUserDetailsDto;

            Assert.NotNull(user);
            Assert.NotNull(tokenValue);
            Assert.Equal(token, tokenValue);
            Assert.Equal(outputUserDetailsDto.Id, user!.Id);
            Assert.Equal(outputUserDetailsDto.Email, user.Email);
            Assert.Equal(outputUserDetailsDto.Username, user.Username);
            Assert.Equal(outputUserDetailsDto.Theme, user.Theme);
        }
        
        [Fact]
        public async Task RegisterUser_ReturnsBadRequest_WithElementNotCreated()
        {
            var registerUserDto = new RegisterUserDto { Email = "user1@example.com", Username = "user1", Password = "password1" };
            OutputUserDetailsDto? outputUserDetailsDto = null;

            _mockUserService.Setup(s => s.RegisterUser(registerUserDto)).ReturnsAsync(outputUserDetailsDto);

            var result = await _controller.RegisterUser(registerUserDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        // POST /User/login-user
        [Fact]
        public async Task LoginUser_ReturnsOk_WithElementLogged()
        {
            var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
            var outputUserDetailsDto = new OutputUserDetailsDto { Id = 1, Email = "user1@example.com", Username = "user1", Theme = "Light" };
            var token = "fake-jwt-token";

            _mockUserService.Setup(s => s.LoginUser(loginUserDto)).ReturnsAsync(outputUserDetailsDto);
            _mockJwt.Setup(j => j.GenerateToken(outputUserDetailsDto.Id)).Returns(token);

            var result = await _controller.LoginUser(loginUserDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);

            var tokenValue = value.GetType().GetProperty("token")?.GetValue(value);
            var user = value.GetType().GetProperty("user")?.GetValue(value) as OutputUserDetailsDto;

            Assert.NotNull(user);
            Assert.NotNull(tokenValue);
            Assert.Equal(token, tokenValue);
            Assert.Equal(outputUserDetailsDto.Id, user!.Id);
            Assert.Equal(outputUserDetailsDto.Email, user.Email);
            Assert.Equal(outputUserDetailsDto.Username, user.Username);
            Assert.Equal(outputUserDetailsDto.Theme, user.Theme);
        }
        
        [Fact]
        public async Task LoginUser_ReturnsBadRequest_WithElementNotLogged()
        {
            var loginUserDto = new LoginUserDto { Email = "user1@example.com", Password = "password1" };
            OutputUserDetailsDto? outputUserDetailsDto = null;

            _mockUserService.Setup(s => s.LoginUser(loginUserDto)).ReturnsAsync(outputUserDetailsDto);

            var result = await _controller.LoginUser(loginUserDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        // PUT /User
        [Fact]
        public async Task UpdateUser_ReturnsOk_WithElementUpdated()
        {
            int userId = 1;
            var updateUserDto = new UpdateUserDto { Username = "user2" };
            var outputUserDetailsDto = new OutputUserDetailsDto { Id = userId, Email = "user1@example.com", Username = "user2", Theme = "Light" };

            _mockUserService.Setup(s => s.UpdateUser(updateUserDto, userId)).ReturnsAsync(outputUserDetailsDto);

            var result = await _controller.UpdateUser(updateUserDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<OutputUserDetailsDto>(okResult.Value);
            Assert.Equal(outputUserDetailsDto.Id, returnedUser.Id);
            Assert.Equal(outputUserDetailsDto.Email, returnedUser.Email);
            Assert.Equal(outputUserDetailsDto.Username, returnedUser.Username);
            Assert.Equal(outputUserDetailsDto.Theme, returnedUser.Theme);
        }
        
        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WithElementNotUpdated()
        {
            int userId = 1;
            var updateUserDto = new UpdateUserDto { Username = "user2" };
            OutputUserDetailsDto? outputUserDetailsDto = null;

            _mockUserService.Setup(s => s.UpdateUser(updateUserDto, userId)).ReturnsAsync(outputUserDetailsDto);

            var result = await _controller.UpdateUser(updateUserDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        // DELETE /User
        [Fact]
        public async Task DeleteUser_ReturnsOk_WithElementDeleted()
        {
            int userId = 1;
            var outputUserDetailsDto = new OutputUserDetailsDto { Id = userId, Email = "user1@example.com", Username = "user2", Theme = "Light" };

            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(false);

            var result = await _controller.DeleteUser();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<OutputUserDetailsDto>(okResult.Value);
            Assert.Equal(outputUserDetailsDto.Id, returnedUser.Id);
            Assert.Equal(outputUserDetailsDto.Email, returnedUser.Email);
            Assert.Equal(outputUserDetailsDto.Username, returnedUser.Username);
            Assert.Equal(outputUserDetailsDto.Theme, returnedUser.Theme);
        }
        
        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WithElementNotDeleted()
        {
            int userId = 1;

            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(false);

            var result = await _controller.DeleteUser();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
