using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers
{
    public class CardLabelControllerTests
    {
        // Renombramos la variable para reflejar que es para "CardLabel"
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
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        [Fact]
        public async Task GetLabelsByCardId_ReturnsOk_WithLabelsFound()
        {
            int userId = 1;
            int cardId = 1;
            var labels = new List<OutputLabelDetailsDto>
            {
                new OutputLabelDetailsDto { Id = 1, Title = "Label 1", Color = "Red", BoardId = 1 },
                new OutputLabelDetailsDto { Id = 2, Title = "Label 2", Color = "Blue", BoardId = 1 }
            };

            _mockCardLabelService
                .Setup(s => s.GetLabelsByCardId(cardId, userId))
                .ReturnsAsync(labels);

            var result = await _controller.GetLabelsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedLabels = Assert.IsType<List<OutputLabelDetailsDto>>(okResult.Value);
            Assert.Equal(labels.Count, returnedLabels.Count);
        }
        
        [Fact]
        public async Task GetLabelsByCardId_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            int cardId = 1;
            var labels = new List<OutputLabelDetailsDto>(); // Lista vacía

            _mockCardLabelService
                .Setup(s => s.GetLabelsByCardId(cardId, userId))
                .ReturnsAsync(labels);

            var result = await _controller.GetLabelsByCardId(cardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedLabels = Assert.IsType<List<OutputLabelDetailsDto>>(okResult.Value);
            Assert.Empty(returnedLabels);
        }
        
        [Fact]
        public async Task AddLabelToCard_ReturnsCreated_WhenLabelIsAdded()
        {
            int userId = 1;
            int cardId = 1;
            // Se asume que AddCardLabelDto tiene al menos la propiedad LabelId
            var dto = new AddCardLabelDto { LabelId = 1 };
            // Se asume que OutputCardLabelDetailsDto contiene CardId y LabelId
            var outputCardLabel = new OutputCardLabelDetailsDto { CardId = cardId, LabelId = dto.LabelId };

            _mockCardLabelService
                .Setup(s => s.AddLabelToCard(cardId, dto, userId))
                .ReturnsAsync(outputCardLabel);

            var result = await _controller.AddLabelToCard(cardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnedCardLabel = Assert.IsType<OutputCardLabelDetailsDto>(createdResult.Value);
            Assert.Equal(outputCardLabel.CardId, returnedCardLabel.CardId);
            Assert.Equal(outputCardLabel.LabelId, returnedCardLabel.LabelId);
        }
        
        [Fact]
        public async Task AddLabelToCard_ReturnsBadRequest_WhenLabelIsNotAdded()
        {
            int userId = 1;
            int cardId = 1;
            var dto = new AddCardLabelDto { LabelId = 1 };

            _mockCardLabelService
                .Setup(s => s.AddLabelToCard(cardId, dto, userId))
                .ReturnsAsync((OutputCardLabelDetailsDto?)null);

            var result = await _controller.AddLabelToCard(cardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveLabelFromCard_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int userId = 1;
            int cardId = 1;
            int labelId = 1;

            _mockCardLabelService
                .Setup(s => s.RemoveLabelFromCard(cardId, labelId, userId))
                .ReturnsAsync(true);

            var result = await _controller.RemoveLabelFromCard(cardId, labelId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveLabelFromCard_ReturnsNotFound_WhenLabelNotFound()
        {
            int userId = 1;
            int cardId = 1;
            int labelId = 1;

            _mockCardLabelService
                .Setup(s => s.RemoveLabelFromCard(cardId, labelId, userId))
                .ReturnsAsync(false);

            var result = await _controller.RemoveLabelFromCard(cardId, labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
