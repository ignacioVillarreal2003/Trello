using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Services
{
    public class LabelServiceTests
    {
        private readonly Mock<ILabelRepository> _mockLabelRepository;
        private readonly Mock<ILogger<LabelService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly LabelService _service;

        public LabelServiceTests()
        {
            _mockLabelRepository = new Mock<ILabelRepository>();
            _mockLogger = new Mock<ILogger<LabelService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            _service = new LabelService(
                _mockMapper.Object,
                _mockBoardAuthorizationService.Object,
                _mockLogger.Object,
                _mockLabelRepository.Object);
        }
        
        [Fact]
        public async Task GetLabelById_ReturnsOutputLabelDetailsDto_WhenLabelExists()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var label = new Label(title: "Label 1", color: "Blue", boardId: 1) { Id = labelId };
            var outputLabelDetailsDto = new OutputLabelDetailsDto 
            { 
                Id = label.Id, 
                Title = label.Title, 
                Color = label.Color 
            };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync(label);
            _mockMapper.Setup(m => m.Map<OutputLabelDetailsDto>(label))
                       .Returns(outputLabelDetailsDto);

            // Act
            var result = await _service.GetLabelById(labelId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputLabelDetailsDto.Id, result.Id);
            Assert.Equal(outputLabelDetailsDto.Title, result.Title);
            Assert.Equal(outputLabelDetailsDto.Color, result.Color);
        }

        [Fact]
        public async Task GetLabelById_ReturnsNull_WhenLabelDoesNotExist()
        {
            // Arrange
            int labelId = 1, userId = 1;
            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync((Label?)null);

            // Act
            var result = await _service.GetLabelById(labelId, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetLabelsByBoardId_ReturnsListOfOutputLabelDetailsDto_WhenLabelsExist()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var labels = new List<Label>
            {
                new Label(title: "Label 1", color: "Blue", boardId: boardId) { Id = 1 },
                new Label(title: "Label 2", color: "Green", boardId: boardId) { Id = 2 }
            };
            var outputLabelDetailsDtos = new List<OutputLabelDetailsDto>
            {
                new OutputLabelDetailsDto { Id = labels[0].Id, Title = labels[0].Title, Color = labels[0].Color },
                new OutputLabelDetailsDto { Id = labels[1].Id, Title = labels[1].Title, Color = labels[1].Color }
            };

            _mockLabelRepository.Setup(r => r.GetLabelsByBoardId(boardId))
                                .ReturnsAsync(labels);
            _mockMapper.Setup(m => m.Map<List<OutputLabelDetailsDto>>(labels))
                       .Returns(outputLabelDetailsDtos);

            // Act
            var result = await _service.GetLabelsByBoardId(boardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(outputLabelDetailsDtos[0].Id, result[0].Id);
            Assert.Equal(outputLabelDetailsDtos[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetLabelsByBoardId_ReturnsEmptyList_WhenNoLabelsExist()
        {
            // Arrange
            int boardId = 1, userId = 1;
            _mockLabelRepository.Setup(r => r.GetLabelsByBoardId(boardId))
                                .ReturnsAsync(new List<Label>());
            _mockMapper.Setup(m => m.Map<List<OutputLabelDetailsDto>>(It.IsAny<List<Label>>()))
                       .Returns(new List<OutputLabelDetailsDto>());

            // Act
            var result = await _service.GetLabelsByBoardId(boardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task AddLabel_ReturnsOutputLabelDetailsDto_WhenLabelIsAdded()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addLabelDto = new AddLabelDto { Title = "New Label", Color = "Blue" };
            var newLabel = new Label(title: addLabelDto.Title, color: addLabelDto.Color, boardId: boardId) { Id = 1 };
            var outputLabelDetailsDto = new OutputLabelDetailsDto 
            { 
                Id = newLabel.Id, 
                Title = newLabel.Title, 
                Color = newLabel.Color 
            };

            _mockLabelRepository.Setup(r => r.AddLabel(It.IsAny<Label>()))
                                .ReturnsAsync(newLabel);
            _mockMapper.Setup(m => m.Map<OutputLabelDetailsDto>(newLabel))
                       .Returns(outputLabelDetailsDto);

            // Act
            var result = await _service.AddLabel(boardId, addLabelDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputLabelDetailsDto.Id, result.Id);
            Assert.Equal(outputLabelDetailsDto.Title, result.Title);
            Assert.Equal(outputLabelDetailsDto.Color, result.Color);
        }

        [Fact]
        public async Task AddLabel_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addLabelDto = new AddLabelDto { Title = "New Label", Color = "Blue" };

            _mockLabelRepository.Setup(r => r.AddLabel(It.IsAny<Label>()))
                                .ReturnsAsync((Label?)null);

            // Act
            var result = await _service.AddLabel(boardId, addLabelDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task UpdateLabel_ReturnsOutputLabelDetailsDto_WhenUpdateIsSuccessful()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = labelId };
            var updateLabelDto = new UpdateLabelDto { Title = "New Title", Color = "Green" };
            var updatedLabel = new Label(title: updateLabelDto.Title, color: updateLabelDto.Color, boardId: existingLabel.BoardId) { Id = existingLabel.Id };
            var outputLabelDetailsDto = new OutputLabelDetailsDto 
            { 
                Id = updatedLabel.Id, 
                Title = updatedLabel.Title, 
                Color = updatedLabel.Color 
            };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync(existingLabel);
            _mockLabelRepository.Setup(r => r.UpdateLabel(existingLabel))
                                .ReturnsAsync(updatedLabel);
            _mockMapper.Setup(m => m.Map<OutputLabelDetailsDto>(updatedLabel))
                       .Returns(outputLabelDetailsDto);

            // Act
            var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputLabelDetailsDto.Id, result.Id);
            Assert.Equal(outputLabelDetailsDto.Title, result.Title);
            Assert.Equal(outputLabelDetailsDto.Color, result.Color);
        }

        [Fact]
        public async Task UpdateLabel_ReturnsNull_WhenLabelNotFound()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var updateLabelDto = new UpdateLabelDto { Title = "New Title", Color = "Green" };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync((Label?)null);

            // Act
            var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateLabel_ReturnsNull_WhenUpdateFails()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = labelId };
            var updateLabelDto = new UpdateLabelDto { Title = "New Title", Color = "Green" };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync(existingLabel);
            _mockLabelRepository.Setup(r => r.UpdateLabel(existingLabel))
                                .ReturnsAsync((Label?)null);

            // Act
            var result = await _service.UpdateLabel(labelId, updateLabelDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task DeleteLabel_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = labelId };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync(existingLabel);
            _mockLabelRepository.Setup(r => r.DeleteLabel(existingLabel))
                                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteLabel(labelId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteLabel_ReturnsFalse_WhenLabelNotFound()
        {
            // Arrange
            int labelId = 1, userId = 1;
            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync((Label?)null);

            // Act
            var result = await _service.DeleteLabel(labelId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteLabel_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int labelId = 1, userId = 1;
            var existingLabel = new Label(title: "Old Title", color: "Blue", boardId: 1) { Id = labelId };

            _mockLabelRepository.Setup(r => r.GetLabelById(labelId))
                                .ReturnsAsync(existingLabel);
            _mockLabelRepository.Setup(r => r.DeleteLabel(existingLabel))
                                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteLabel(labelId, userId);

            // Assert
            Assert.False(result);
        }
    }
}
