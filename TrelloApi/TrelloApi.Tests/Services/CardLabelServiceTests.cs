using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.DTOs;

namespace TrelloApi.Tests.Services
{
    public class CardLabelServiceTests
    {
        private readonly Mock<ICardLabelRepository> _mockCardLabelRepository;
        private readonly Mock<ILogger<CardLabelService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly CardLabelService _service;

        public CardLabelServiceTests()
        {
            _mockCardLabelRepository = new Mock<ICardLabelRepository>();
            _mockLogger = new Mock<ILogger<CardLabelService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            _service = new CardLabelService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockLogger.Object,
                _mockCardLabelRepository.Object);
        }

        [Fact]
        public async Task GetLabelsByCardId_ReturnsListOfOutputLabelDetailsDto_WhenLabelsExist()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var labels = new List<Label>
            {
                new Label("title", "color", 1) // Completar según la implementación real de Label
            };
            var outputDtos = new List<OutputLabelDetailsDto>
            {
                new OutputLabelDetailsDto() // Completar propiedades según sea necesario
            };

            _mockCardLabelRepository
                .Setup(r => r.GetLabelsByCardId(cardId))
                .ReturnsAsync(labels);
            _mockMapper
                .Setup(m => m.Map<List<OutputLabelDetailsDto>>(labels))
                .Returns(outputDtos);

            // Act
            var result = await _service.GetLabelsByCardId(cardId, uid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDtos.Count, result.Count);
            // Opcional: comparar propiedades de cada DTO
        }

        [Fact]
        public async Task GetLabelsByCardId_ReturnsEmptyList_WhenNoLabelsExist()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var labels = new List<Label>();
            var outputDtos = new List<OutputLabelDetailsDto>();

            _mockCardLabelRepository
                .Setup(r => r.GetLabelsByCardId(cardId))
                .ReturnsAsync(labels);
            _mockMapper
                .Setup(m => m.Map<List<OutputLabelDetailsDto>>(labels))
                .Returns(outputDtos);

            // Act
            var result = await _service.GetLabelsByCardId(cardId, uid);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddLabelToCard_ReturnsOutputCardLabelDetailsDto_WhenLabelIsAdded()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var addDto = new AddCardLabelDto { LabelId = 1 };
            var newCardLabel = new CardLabel(cardId, addDto.LabelId);
            var outputDto = new OutputCardLabelDetailsDto
            {
                // Asigna las propiedades según la implementación (por ejemplo, CardId, LabelId, etc.)
            };

            _mockCardLabelRepository
                .Setup(r => r.AddCardLabel(It.IsAny<CardLabel>()))
                ;
            _mockMapper
                .Setup(m => m.Map<OutputCardLabelDetailsDto>(newCardLabel))
                .Returns(outputDto);

            // Act
            var result = await _service.AddLabelToCard(cardId, addDto, uid);

            // Assert
            Assert.NotNull(result);
            // Ejemplo de comprobación (ajustar según las propiedades reales)
            // Assert.Equal(outputDto.CardId, result.CardId);
            // Assert.Equal(outputDto.LabelId, result.LabelId);
        }

        [Fact]
        public async Task AddLabelToCard_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int cardId = 1, uid = 1;
            var addDto = new AddCardLabelDto { LabelId = 1 };

            _mockCardLabelRepository
                .Setup(r => r.AddCardLabel(It.IsAny<CardLabel>()))
                ;

            // Act
            var result = await _service.AddLabelToCard(cardId, addDto, uid);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveLabelFromCard_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int cardId = 1, labelId = 1, uid = 1;
            var existingCardLabel = new CardLabel(cardId, labelId);
            _mockCardLabelRepository
                .Setup(r => r.GetCardLabelById(cardId, labelId))
                .ReturnsAsync(existingCardLabel);
            _mockCardLabelRepository
                .Setup(r => r.DeleteCardLabel(existingCardLabel))
                ;

            // Act
            var result = await _service.RemoveLabelFromCard(cardId, labelId, uid);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveLabelFromCard_ReturnsFalse_WhenLabelNotFound()
        {
            // Arrange
            int cardId = 1, labelId = 1, uid = 1;
            _mockCardLabelRepository
                .Setup(r => r.GetCardLabelById(cardId, labelId))
                .ReturnsAsync((CardLabel?)null);

            // Act
            var result = await _service.RemoveLabelFromCard(cardId, labelId, uid);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveLabelFromCard_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int cardId = 1, labelId = 1, uid = 1;
            var existingCardLabel = new CardLabel(cardId, labelId);
            _mockCardLabelRepository
                .Setup(r => r.GetCardLabelById(cardId, labelId))
                .ReturnsAsync(existingCardLabel);
            _mockCardLabelRepository
                .Setup(r => r.DeleteCardLabel(existingCardLabel))
                ;

            // Act
            var result = await _service.RemoveLabelFromCard(cardId, labelId, uid);

            // Assert
            Assert.False(result);
        }
    }
}
