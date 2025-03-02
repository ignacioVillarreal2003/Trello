using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Tests.Controllers
{
    public class CardLabelControllerTests
    {
        private readonly Mock<ICardLabelService> _mockCardLabelService;
        private readonly Mock<ILogger<CardLabelController>> _mockLogger;
        private readonly CardLabelController _controller;

        public CardLabelControllerTests()
        {
            _mockCardLabelService = new Mock<ICardLabelService>();
            _mockLogger = new Mock<ILogger<CardLabelController>>();

            _controller = new CardLabelController(_mockLogger.Object, _mockCardLabelService.Object);
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
        public async Task GetLabelsByCardId_ShouldReturnsOk_WithLabelsFound()
        {
            const int cardId = 1;
            var response = new List<LabelResponse>
            {
                new LabelResponse { Id = 1, Title = "title 1", Color = "color", BoardId = 1 },
                new LabelResponse { Id = 2, Title = "title 2", Color = "color", BoardId = 1 }
            };

            _mockCardLabelService.Setup(s => s.GetLabelsByCardId(cardId)).ReturnsAsync(response);

            var result = await _controller.GetLabelsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<LabelResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetLabelsByCardId_ShouldReturnsOk_WithLabelsNotFound()
        {
            const int cardId = 1;

            _mockCardLabelService.Setup(s => s.GetLabelsByCardId(cardId)).ReturnsAsync([]);

            var result = await _controller.GetLabelsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<LabelResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task AddLabelToCard_ShouldReturnsCreated_WhenAddedSuccessfully()
        {
            const int cardId = 1;
            var dto = new AddCardLabelDto { LabelId = 1 };
            var response = new CardLabelResponse { CardId = cardId, LabelId = dto.LabelId };

            _mockCardLabelService.Setup(s => s.AddLabelToCard(cardId, dto)).ReturnsAsync(response);

            var result = await _controller.AddLabelToCard(cardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddLabelToCard_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int cardId = 1;
            var dto = new AddCardLabelDto { LabelId = 1 };

            _mockCardLabelService.Setup(s => s.AddLabelToCard(cardId, dto)).ReturnsAsync((CardLabelResponse?)null);

            var result = await _controller.AddLabelToCard(cardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveLabelFromCard_ShouldReturnsNoContent_WhenDeleteSuccessful()
        {
            const int cardId = 1, labelId = 1;

            _mockCardLabelService.Setup(s => s.RemoveLabelFromCard(cardId, labelId)).ReturnsAsync(true);

            var result = await _controller.RemoveLabelFromCard(cardId, labelId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveLabelFromCard_ShouldReturnsNotFound_WhenDeleteUnsuccessful()
        {
            const int cardId = 1, labelId = 1;

            _mockCardLabelService.Setup(s => s.RemoveLabelFromCard(cardId, labelId)).ReturnsAsync(false);

            var result = await _controller.RemoveLabelFromCard(cardId, labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
