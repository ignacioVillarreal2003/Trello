using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
using Xunit;

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
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        // --- Tests para GET ---
        [Fact]
        public async Task GetUsersByBoardId_ReturnsOk_WithUsersFound()
        {
            int currentUserId = 1;
            int boardId = 1;
            var users = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Email = "user1@test.com", Username = "user1", Theme = "light" },
                new OutputUserDetailsDto { Id = 2, Email = "user2@test.com", Username = "user2", Theme = "dark" }
            };

            _mockUserBoardService
                .Setup(s => s.GetUsersByBoardId(boardId, currentUserId))
                .ReturnsAsync(users);

            var result = await _controller.GetUsersByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUsers = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count);
        }
        
        [Fact]
        public async Task GetUsersByBoardId_ReturnsOk_WithEmptyList()
        {
            int currentUserId = 1;
            int boardId = 1;
            var users = new List<OutputUserDetailsDto>();

            _mockUserBoardService
                .Setup(s => s.GetUsersByBoardId(boardId, currentUserId))
                .ReturnsAsync(users);

            var result = await _controller.GetUsersByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUsers = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }
        
        // --- Tests para POST ---
        [Fact]
        public async Task AddUserToBoard_ReturnsCreated_WhenUserIsAdded()
        {
            int currentUserId = 1;
            int boardId = 1;
            // Suponemos que para agregar un usuario a un board se usa el DTO AddUserBoardDto
            // y que el servicio retorna un OutputUserBoardDetailsDto.
            var addUserBoardDto = new AddUserBoardDto { UserId = 2, Role = "Member" };
            var outputUserBoard = new OutputUserBoardDetailsDto { UserId = 2, BoardId = boardId };

            _mockUserBoardService
                .Setup(s => s.AddUserToBoard(boardId, addUserBoardDto, currentUserId))
                .ReturnsAsync(outputUserBoard);

            var result = await _controller.AddUserToBoard(boardId, addUserBoardDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnedUserBoard = Assert.IsType<OutputUserBoardDetailsDto>(createdResult.Value);
            Assert.Equal(outputUserBoard.UserId, returnedUserBoard.UserId);
            Assert.Equal(outputUserBoard.BoardId, returnedUserBoard.BoardId);
        }
        
        [Fact]
        public async Task AddUserToBoard_ReturnsBadRequest_WhenUserNotAdded()
        {
            int currentUserId = 1;
            int boardId = 1;
            var addUserBoardDto = new AddUserBoardDto { UserId = 2, Role = "Member" };

            _mockUserBoardService
                .Setup(s => s.AddUserToBoard(boardId, addUserBoardDto, currentUserId))
                .ReturnsAsync((OutputUserBoardDetailsDto?)null);

            var result = await _controller.AddUserToBoard(boardId, addUserBoardDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        // --- Tests para DELETE ---
        [Fact]
        public async Task RemoveUserFromBoard_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int currentUserId = 1;
            int boardId = 1;
            int userIdToRemove = 2;

            _mockUserBoardService
                .Setup(s => s.RemoveUserFromBoard(boardId, userIdToRemove, currentUserId))
                .ReturnsAsync(true);

            var result = await _controller.RemoveUserFromBoard(boardId, userIdToRemove);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromBoard_ReturnsNotFound_WhenDeletionFails()
        {
            int currentUserId = 1;
            int boardId = 1;
            int userIdToRemove = 2;

            _mockUserBoardService
                .Setup(s => s.RemoveUserFromBoard(boardId, userIdToRemove, currentUserId))
                .ReturnsAsync(false);

            var result = await _controller.RemoveUserFromBoard(boardId, userIdToRemove);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
