using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;
using Xunit;

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
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        // Test para obtener los usuarios asignados a una tarjeta
        [Fact]
        public async Task GetUsersByCardId_ReturnsOk_WithUsersFound()
        {
            int currentUserId = 1;
            int cardId = 1;
            var users = new List<OutputUserDetailsDto>
            {
                new OutputUserDetailsDto { Id = 1, Email = "user1@test.com", Username = "user1", Theme = "light" },
                new OutputUserDetailsDto { Id = 2, Email = "user2@test.com", Username = "user2", Theme = "dark" }
            };

            _mockUserCardService
                .Setup(s => s.GetUsersByCardId(cardId, currentUserId))
                .ReturnsAsync(users);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUsers = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count);
        }
        
        [Fact]
        public async Task GetUsersByCardId_ReturnsOk_WithEmptyList()
        {
            int currentUserId = 1;
            int cardId = 1;
            var users = new List<OutputUserDetailsDto>();

            _mockUserCardService
                .Setup(s => s.GetUsersByCardId(cardId, currentUserId))
                .ReturnsAsync(users);

            var result = await _controller.GetUsersByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedUsers = Assert.IsType<List<OutputUserDetailsDto>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }
        
        // Test para agregar un usuario a una tarjeta
        [Fact]
        public async Task AddUserToCard_ReturnsCreated_WhenUserIsAdded()
        {
            int currentUserId = 1;
            int cardId = 1;
            var addUserCardDto = new AddUserCardDto { UserId = 2 };
            var outputUserCard = new OutputUserCardDetailsDto { UserId = 2, CardId = cardId };

            _mockUserCardService
                .Setup(s => s.AddUserToCard(cardId, addUserCardDto, currentUserId))
                .ReturnsAsync(outputUserCard);

            var result = await _controller.AddUserToCard(cardId, addUserCardDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnedUserCard = Assert.IsType<OutputUserCardDetailsDto>(createdResult.Value);
            Assert.Equal(outputUserCard.UserId, returnedUserCard.UserId);
            Assert.Equal(outputUserCard.CardId, returnedUserCard.CardId);
        }
        
        [Fact]
        public async Task AddUserToCard_ReturnsBadRequest_WhenUserNotAdded()
        {
            int currentUserId = 1;
            int cardId = 1;
            var addUserCardDto = new AddUserCardDto { UserId = 2 };

            _mockUserCardService
                .Setup(s => s.AddUserToCard(cardId, addUserCardDto, currentUserId))
                .ReturnsAsync((OutputUserCardDetailsDto?)null);

            var result = await _controller.AddUserToCard(cardId, addUserCardDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        // Test para eliminar un usuario de una tarjeta
        [Fact]
        public async Task RemoveUserFromCard_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int currentUserId = 1;
            int userIdToRemove = 2;
            int cardId = 1;

            _mockUserCardService
                .Setup(s => s.RemoveUserFromCard(userIdToRemove, cardId, currentUserId))
                .ReturnsAsync(true);

            var result = await _controller.RemoveUserFromCard(userIdToRemove, cardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveUserFromCard_ReturnsNotFound_WhenDeletionFails()
        {
            int currentUserId = 1;
            int userIdToRemove = 2;
            int cardId = 1;

            _mockUserCardService
                .Setup(s => s.RemoveUserFromCard(userIdToRemove, cardId, currentUserId))
                .ReturnsAsync(false);

            var result = await _controller.RemoveUserFromCard(userIdToRemove, cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
