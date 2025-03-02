using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Card;

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
        public async Task GetCardById_ShouldReturnsOk_WhenCardFound()
        {
            const int cardId = 1;
            var response = new CardResponse
            { 
                Id = cardId,
                Title = "title",
                Description = "description",
                ListId = 1,
                DueDate = null,
                Priority = "priority",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockCardService.Setup(s => s.GetCardById(cardId)).ReturnsAsync(response);

            var result = await _controller.GetCardById(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetCardById_ShouldReturnsNotFound_WhenCardNotFound()
        {
            const int cardId = 1;

            _mockCardService.Setup(s => s.GetCardById(cardId)).ReturnsAsync((CardResponse?)null);

            var result = await _controller.GetCardById(cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetCardsByListId_ShouldReturnsOk_WhenCardsFound()
        {
            const int listId = 1;
            var response = new List<CardResponse>
            {
                new CardResponse { Id = 1, Title = "title 1", Description = "description 1", ListId = listId, Priority = "priority", IsCompleted = false, CreatedAt = DateTime.UtcNow },
                new CardResponse { Id = 2, Title = "title 2", Description = "description 2", ListId = listId, Priority = "priority", IsCompleted = false, CreatedAt = DateTime.UtcNow }
            };

            _mockCardService.Setup(s => s.GetCardsByListId(listId)).ReturnsAsync(response);

            var result = await _controller.GetCardsByListId(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<CardResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }

        [Fact]
        public async Task GetCardsByListId_ShouldReturnsOk_WhenCardsNotFound()
        {
            const int listId = 1;

            _mockCardService.Setup(s => s.GetCardsByListId(listId)).ReturnsAsync([]);

            var result = await _controller.GetCardsByListId(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<CardResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }

        [Fact]
        public async Task GetPriorities_ShouldReturnsOk_WhenPrioritiesFound()
        {
            var result = await _controller.GetPriorities();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<string>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task AddCard_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int listId = 1;
            var dto = new AddCardDto { Title = "title", Description = "description", Priority = "priority" };
            var response = new CardResponse
            {
                Id = 1,
                Title = dto.Title,
                Description = dto.Description,
                ListId = listId,
                Priority = dto.Priority,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockCardService.Setup(s => s.AddCard(listId, dto)).ReturnsAsync(response);

            var result = await _controller.AddCard(listId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task AddCard_ReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int listId = 1;
            var dto = new AddCardDto { Title = "title", Description = "description", Priority = "priority" };

            _mockCardService.Setup(s => s.AddCard(listId, dto)).ReturnsAsync((CardResponse?)null);

            var result = await _controller.AddCard(listId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCard_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int cardId = 1;
            var dto = new UpdateCardDto { Title = "updated title" };
            var response = new CardResponse
            {
                Id = cardId,
                Title = dto.Title,
                Description = "description",
                ListId = 1,
                Priority = "priority",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                DueDate = null
            };

            _mockCardService.Setup(s => s.UpdateCard(cardId, dto)).ReturnsAsync(response);

            var result = await _controller.UpdateCard(cardId, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCard_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int cardId = 1;
            var updateCardDto = new UpdateCardDto { Title = "updated title" };

            _mockCardService.Setup(s => s.UpdateCard(cardId, updateCardDto)).ReturnsAsync((CardResponse?)null);

            var result = await _controller.UpdateCard(cardId, updateCardDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCard_ShouldReturnsNoContent_WhenDeleteSuccessful()
        {
            const int cardId = 1;

            _mockCardService.Setup(s => s.DeleteCard(cardId)).ReturnsAsync(true);

            var result = await _controller.DeleteCard(cardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCard_ShouldReturnsNotFound_WhenDeleteUnsuccessful()
        {
            const int cardId = 1;
            _mockCardService.Setup(s => s.DeleteCard(cardId)).ReturnsAsync(false);

            var result = await _controller.DeleteCard(cardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
