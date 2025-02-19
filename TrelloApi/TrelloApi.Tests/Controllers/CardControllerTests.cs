using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs; // OutputCardDetailsDto, AddCardDto, UpdateCardDto
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers
{
    public class CardControllerTests
    {
        private readonly Mock<ICardService> _mockCardService;
        private readonly Mock<ILogger<CardController>> _mockLogger;
        private readonly CardController _controller;

        public CardControllerTests()
        {
            _mockCardService = new Mock<ICardService>();
            _mockLogger = new Mock<ILogger<CardController>>();
            _controller = new CardController(_mockLogger.Object, _mockCardService.Object);
            SetUserId(1);
        }

        private void SetUserId(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetCardById_ReturnsOk_WithElementFound()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;
            var outputCardDto = new OutputCardDetailsDto 
            { 
                Id = cardId,
                Title = "Card 1",
                Description = "Test card",
                ListId = 1,
                DueDate = null,
                Priority = "Medium",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockCardService.Setup(s => s.GetCardById(cardId, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.GetCardById(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<OutputCardDetailsDto>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(outputCardDto.Id, value.Id);
            Assert.Equal(outputCardDto.Title, value.Title);
            Assert.Equal(outputCardDto.Description, value.Description);
            Assert.Equal(outputCardDto.Priority, value.Priority);
            Assert.Equal(outputCardDto.IsCompleted, value.IsCompleted);
        }

        [Fact]
        public async Task GetCardById_ReturnsNotFound_WhenElementNotFound()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;
            OutputCardDetailsDto? outputCardDto = null;

            _mockCardService.Setup(s => s.GetCardById(cardId, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.GetCardById(cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetCardsByListId_ReturnsOk_WithFullList()
        {
            // Arrange
            int userId = 1;
            int listId = 1;
            var listOutputCardDto = new List<OutputCardDetailsDto>
            {
                new OutputCardDetailsDto { Id = 1, Title = "Card 1", Description = "", ListId = listId, Priority = "Medium", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new OutputCardDetailsDto { Id = 2, Title = "Card 2", Description = "", ListId = listId, Priority = "High", IsCompleted = false, CreatedAt = DateTime.UtcNow }
            };

            _mockCardService.Setup(s => s.GetCardsByListId(listId, userId))
                            .ReturnsAsync(listOutputCardDto);

            // Act
            var result = await _controller.GetCardsByListId(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<OutputCardDetailsDto>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(2, value.Count);
        }

        [Fact]
        public async Task GetCardsByListId_ReturnsOk_WithEmptyList()
        {
            // Arrange
            int userId = 1;
            int listId = 1;
            var listOutputCardDto = new List<OutputCardDetailsDto>();

            _mockCardService.Setup(s => s.GetCardsByListId(listId, userId))
                            .ReturnsAsync(listOutputCardDto);

            // Act
            var result = await _controller.GetCardsByListId(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<OutputCardDetailsDto>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }

        [Fact]
        public async Task GetPriorities_ReturnsOk_WithPrioritiesList()
        {
            // Arrange
            // Este endpoint usa la ruta "priorities" y retorna la lista de prioridades definida en PriorityValues.
            // Act
            var result = await _controller.GetBoardColors();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<string>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task AddCard_ReturnsCreatedAtAction_WithElementCreated()
        {
            // Arrange
            int userId = 1;
            int listId = 1;
            var addCardDto = new AddCardDto { Title = "Card 1", Description = "Test card", Priority = "Medium" };
            var outputCardDto = new OutputCardDetailsDto
            {
                Id = 1,
                Title = "Card 1",
                Description = "Test card",
                ListId = listId,
                Priority = "Medium",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockCardService.Setup(s => s.AddCard(listId, addCardDto, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.AddCard(listId, addCardDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsType<OutputCardDetailsDto>(createdResult.Value);

            // Assert
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(outputCardDto.Id, value.Id);
            Assert.Equal(outputCardDto.Title, value.Title);
            Assert.Equal(outputCardDto.Description, value.Description);
            Assert.Equal(outputCardDto.Priority, value.Priority);
            Assert.Equal(outputCardDto.IsCompleted, value.IsCompleted);
        }

        [Fact]
        public async Task AddCard_ReturnsBadRequest_WithElementNotCreated()
        {
            // Arrange
            int userId = 1;
            int listId = 1;
            var addCardDto = new AddCardDto { Title = "Card 1", Description = "Test card", Priority = "Medium" };
            OutputCardDetailsDto? outputCardDto = null;

            _mockCardService.Setup(s => s.AddCard(listId, addCardDto, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.AddCard(listId, addCardDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Assert
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCard_ReturnsOk_WithElementUpdated()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;
            var updateCardDto = new UpdateCardDto { Title = "Updated Card" };
            var outputCardDto = new OutputCardDetailsDto
            {
                Id = cardId,
                Title = "Updated Card",
                Description = "Test card",
                ListId = 1,
                Priority = "Medium",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockCardService.Setup(s => s.UpdateCard(cardId, updateCardDto, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.UpdateCard(cardId, updateCardDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<OutputCardDetailsDto>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(outputCardDto.Id, value.Id);
            Assert.Equal(outputCardDto.Title, value.Title);
        }

        [Fact]
        public async Task UpdateCard_ReturnsNotFound_WithElementNotUpdated()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;
            var updateCardDto = new UpdateCardDto { Title = "Updated Card" };
            OutputCardDetailsDto? outputCardDto = null;

            _mockCardService.Setup(s => s.UpdateCard(cardId, updateCardDto, userId))
                            .ReturnsAsync(outputCardDto);

            // Act
            var result = await _controller.UpdateCard(cardId, updateCardDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCard_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;

            _mockCardService.Setup(s => s.DeleteCard(cardId, userId))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCard(cardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);

            // Assert
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCard_ReturnsNotFound_WhenElementNotDeleted()
        {
            // Arrange
            int userId = 1;
            int cardId = 1;
            _mockCardService.Setup(s => s.DeleteCard(cardId, userId))
                            .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCard(cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
