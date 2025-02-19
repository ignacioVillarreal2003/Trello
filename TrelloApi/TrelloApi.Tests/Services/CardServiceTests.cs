using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Xunit;

namespace TrelloApi.Tests.Services
{
    public class CardServiceTests
    {
        private readonly Mock<ICardRepository> _mockCardRepository;
        private readonly Mock<ILogger<CardService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly CardService _service;

        public CardServiceTests()
        {
            _mockCardRepository = new Mock<ICardRepository>();
            _mockLogger = new Mock<ILogger<CardService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            _service = new CardService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockLogger.Object,
                _mockCardRepository.Object);
        }
        
        [Fact]
        public async Task GetCardById_ReturnsOutputCardDetailsDto_WhenCardExists()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var card = new Card("Card Title", "Card Description", listId: 1, priority: "Medium") { Id = cardId };
            var outputDto = new OutputCardDetailsDto
            {
                // Ajusta las propiedades según tu implementación
                Id = card.Id,
                Title = card.Title,
                Description = card.Description,
                Priority = card.Priority
            };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync(card);
            _mockMapper.Setup(m => m.Map<OutputCardDetailsDto>(card))
                       .Returns(outputDto);

            // Act
            var result = await _service.GetCardById(cardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Description, result.Description);
            Assert.Equal(outputDto.Priority, result.Priority);
        }
        
        [Fact]
        public async Task GetCardById_ReturnsNull_WhenCardDoesNotExist()
        {
            // Arrange
            int cardId = 1, userId = 1;
            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync((Card?)null);

            // Act
            var result = await _service.GetCardById(cardId, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCardsByListId_ReturnsListOfOutputCardDetailsDto_WhenCardsExist()
        {
            // Arrange
            int listId = 1, userId = 1;
            var cards = new List<Card>
            {
                new Card("Card 1", "Description 1", listId, "Medium") { Id = 1 },
                new Card("Card 2", "Description 2", listId, "High") { Id = 2 }
            };
            var outputDtos = new List<OutputCardDetailsDto>
            {
                new OutputCardDetailsDto { Id = 1, Title = "Card 1", Description = "Description 1", Priority = "Medium" },
                new OutputCardDetailsDto { Id = 2, Title = "Card 2", Description = "Description 2", Priority = "High" }
            };

            _mockCardRepository.Setup(r => r.GetCardsByListId(listId))
                               .ReturnsAsync(cards);
            _mockMapper.Setup(m => m.Map<List<OutputCardDetailsDto>>(cards))
                       .Returns(outputDtos);

            // Act
            var result = await _service.GetCardsByListId(listId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDtos.Count, result.Count);
        }

        [Fact]
        public async Task GetCardsByListId_ReturnsEmptyList_WhenNoCardsExist()
        {
            // Arrange
            int listId = 1, userId = 1;
            var cards = new List<Card>();
            var outputDtos = new List<OutputCardDetailsDto>();

            _mockCardRepository.Setup(r => r.GetCardsByListId(listId))
                               .ReturnsAsync(cards);
            _mockMapper.Setup(m => m.Map<List<OutputCardDetailsDto>>(cards))
                       .Returns(outputDtos);

            // Act
            var result = await _service.GetCardsByListId(listId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task AddCard_ReturnsOutputCardDetailsDto_WhenCardIsAdded()
        {
            // Arrange
            int listId = 1, userId = 1;
            var addCardDto = new AddCardDto { Title = "New Card", Description = "New Description", Priority = "Low" };
            var newCard = new Card(addCardDto.Title, addCardDto.Description, listId, addCardDto.Priority) { Id = 1 };
            var outputDto = new OutputCardDetailsDto
            {
                Id = newCard.Id,
                Title = newCard.Title,
                Description = newCard.Description,
                Priority = newCard.Priority
            };

            _mockCardRepository.Setup(r => r.AddCard(It.IsAny<Card>()));
            _mockMapper.Setup(m => m.Map<OutputCardDetailsDto>(newCard))
                       .Returns(outputDto);

            // Act
            var result = await _service.AddCard(listId, addCardDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Description, result.Description);
            Assert.Equal(outputDto.Priority, result.Priority);
        }

        [Fact]
        public async Task AddCard_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int listId = 1, userId = 1;
            var addCardDto = new AddCardDto { Title = "New Card", Description = "New Description", Priority = "Low" };

            _mockCardRepository.Setup(r => r.AddCard(It.IsAny<Card>()));

            // Act
            var result = await _service.AddCard(listId, addCardDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task UpdateCard_ReturnsOutputCardDetailsDto_WhenUpdateIsSuccessful()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var existingCard = new Card("Old Title", "Old Description", listId: 1, priority: "Medium") { Id = cardId };
            var updateCardDto = new UpdateCardDto { Title = "New Title", Description = "New Description", Priority = "High" };
            // Simula la actualización: se crean una nueva instancia con las propiedades actualizadas.
            var updatedCard = new Card(updateCardDto.Title, updateCardDto.Description, existingCard.ListId, updateCardDto.Priority) { Id = cardId };
            var outputDto = new OutputCardDetailsDto
            {
                Id = updatedCard.Id,
                Title = updatedCard.Title,
                Description = updatedCard.Description,
                Priority = updatedCard.Priority
            };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync(existingCard);
            _mockCardRepository.Setup(r => r.UpdateCard(existingCard));
            _mockMapper.Setup(m => m.Map<OutputCardDetailsDto>(updatedCard))
                       .Returns(outputDto);

            // Act
            var result = await _service.UpdateCard(cardId, updateCardDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Description, result.Description);
            Assert.Equal(outputDto.Priority, result.Priority);
        }

        [Fact]
        public async Task UpdateCard_ReturnsNull_WhenCardNotFound()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var updateCardDto = new UpdateCardDto { Title = "New Title" };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync((Card?)null);

            // Act
            var result = await _service.UpdateCard(cardId, updateCardDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCard_ReturnsNull_WhenUpdateFails()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var existingCard = new Card("Old Title", "Old Description", listId: 1, priority: "Medium") { Id = cardId };
            var updateCardDto = new UpdateCardDto { Title = "New Title" };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync(existingCard);
            _mockCardRepository.Setup(r => r.UpdateCard(existingCard));

            // Act
            var result = await _service.UpdateCard(cardId, updateCardDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task DeleteCard_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var existingCard = new Card("Title", "Description", listId: 1, priority: "Medium") { Id = cardId };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync(existingCard);
            _mockCardRepository.Setup(r => r.DeleteCard(existingCard));

            // Act
            var result = await _service.DeleteCard(cardId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCard_ReturnsFalse_WhenCardNotFound()
        {
            // Arrange
            int cardId = 1, userId = 1;
            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync((Card?)null);

            // Act
            var result = await _service.DeleteCard(cardId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCard_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int cardId = 1, userId = 1;
            var existingCard = new Card("Title", "Description", listId: 1, priority: "Medium") { Id = cardId };

            _mockCardRepository.Setup(r => r.GetCardById(cardId))
                               .ReturnsAsync(existingCard);
            _mockCardRepository.Setup(r => r.DeleteCard(existingCard));

            // Act
            var result = await _service.DeleteCard(cardId, userId);

            // Assert
            Assert.False(result);
        }
    }
}
