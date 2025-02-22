using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrelloApi.Application.Services.Interfaces;

namespace TrelloApi.Tests.Controllers
{
    public class LabelControllerTests
    {
        private readonly Mock<ILabelService> _mockLabelService;
        private readonly Mock<ILogger<LabelController>> _mockLogger;
        private readonly LabelController _controller;

        public LabelControllerTests()
        {
            _mockLabelService = new Mock<ILabelService>();
            _mockLogger = new Mock<ILogger<LabelController>>();
            _controller = new LabelController(_mockLogger.Object, _mockLabelService.Object);
            SetUserId(1);
        }
        
        private void SetUserId(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        [Fact]
        public async Task GetLabelById_ReturnsOk_WhenLabelFound()
        {
            int userId = 1;
            int labelId = 1;
            var outputLabel = new OutputLabelDetailsDto 
            { 
                Id = labelId, 
                Title = "Label 1", 
                Color = "Blue", 
                BoardId = 1 
            };

            _mockLabelService
                .Setup(s => s.GetLabelById(labelId, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.GetLabelById(labelId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLabel = Assert.IsType<OutputLabelDetailsDto>(okResult.Value);
            Assert.Equal(outputLabel.Id, returnedLabel.Id);
            Assert.Equal(outputLabel.Title, returnedLabel.Title);
            Assert.Equal(outputLabel.Color, returnedLabel.Color);
            Assert.Equal(outputLabel.BoardId, returnedLabel.BoardId);
        }
        
        [Fact]
        public async Task GetLabelById_ReturnsNotFound_WhenLabelNotFound()
        {
            int userId = 1;
            int labelId = 1;
            OutputLabelDetailsDto? outputLabel = null;
            
            _mockLabelService
                .Setup(s => s.GetLabelById(labelId, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.GetLabelById(labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetLabelsByBoardId_ReturnsOk_WithLabels()
        {
            int userId = 1;
            int boardId = 1;
            var labels = new List<OutputLabelDetailsDto>
            {
                new OutputLabelDetailsDto { Id = 1, Title = "Label 1", Color = "Blue", BoardId = boardId },
                new OutputLabelDetailsDto { Id = 2, Title = "Label 2", Color = "Red", BoardId = boardId }
            };

            _mockLabelService
                .Setup(s => s.GetLabelsByBoardId(boardId, userId))
                .ReturnsAsync(labels);

            var result = await _controller.GetLabelsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLabels = Assert.IsType<List<OutputLabelDetailsDto>>(okResult.Value);
            Assert.Equal(labels.Count, returnedLabels.Count);
        }
        
        [Fact]
        public async Task GetLabelsByBoardId_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            int boardId = 1;
            var labels = new List<OutputLabelDetailsDto>();

            _mockLabelService
                .Setup(s => s.GetLabelsByBoardId(boardId, userId))
                .ReturnsAsync(labels);

            var result = await _controller.GetLabelsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLabels = Assert.IsType<List<OutputLabelDetailsDto>>(okResult.Value);
            Assert.Empty(returnedLabels);
        }
        
        [Fact]
        public async Task GetLabelColors_ReturnsOk_WithColors()
        {
            // El endpoint GetLabelColors devuelve los colores permitidos (definidos estáticamente).
            var result = await _controller.GetLabelColors();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedColors = Assert.IsType<List<string>>(okResult.Value);
            Assert.NotNull(returnedColors);
            // Opcional: Validar que la lista contenga elementos.
            Assert.NotEmpty(returnedColors);
        }
        
        [Fact]
        public async Task AddLabel_ReturnsCreated_WhenLabelIsAdded()
        {
            int userId = 1;
            int boardId = 1;
            var addLabelDto = new AddLabelDto 
            { 
                Title = "Label 1", 
                Color = "Blue", 
                BoardId = boardId 
            };
            var outputLabel = new OutputLabelDetailsDto 
            { 
                Id = 1, 
                Title = "Label 1", 
                Color = "Blue", 
                BoardId = boardId 
            };

            _mockLabelService
                .Setup(s => s.AddLabel(boardId, addLabelDto, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.AddLabel(boardId, addLabelDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnedLabel = Assert.IsType<OutputLabelDetailsDto>(createdResult.Value);
            Assert.Equal(outputLabel.Id, returnedLabel.Id);
            Assert.Equal(outputLabel.Title, returnedLabel.Title);
            Assert.Equal(outputLabel.Color, returnedLabel.Color);
            Assert.Equal(outputLabel.BoardId, returnedLabel.BoardId);
        }
        
        [Fact]
        public async Task AddLabel_ReturnsBadRequest_WhenLabelNotAdded()
        {
            int userId = 1;
            int boardId = 1;
            var addLabelDto = new AddLabelDto 
            { 
                Title = "Label 1", 
                Color = "Blue", 
                BoardId = boardId 
            };
            OutputLabelDetailsDto? outputLabel = null;
            
            _mockLabelService
                .Setup(s => s.AddLabel(boardId, addLabelDto, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.AddLabel(boardId, addLabelDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateLabel_ReturnsOk_WhenLabelIsUpdated()
        {
            int userId = 1;
            int labelId = 1;
            var updateLabelDto = new UpdateLabelDto 
            { 
                Title = "Updated Label", 
                Color = "Red" 
            };
            var outputLabel = new OutputLabelDetailsDto 
            { 
                Id = labelId, 
                Title = "Updated Label", 
                Color = "Red", 
                BoardId = 1 
            };

            _mockLabelService
                .Setup(s => s.UpdateLabel(labelId, updateLabelDto, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.UpdateLabel(labelId, updateLabelDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLabel = Assert.IsType<OutputLabelDetailsDto>(okResult.Value);
            Assert.Equal(outputLabel.Id, returnedLabel.Id);
            Assert.Equal(outputLabel.Title, returnedLabel.Title);
            Assert.Equal(outputLabel.Color, returnedLabel.Color);
            Assert.Equal(outputLabel.BoardId, returnedLabel.BoardId);
        }
        
        [Fact]
        public async Task UpdateLabel_ReturnsNotFound_WhenLabelNotFound()
        {
            int userId = 1;
            int labelId = 1;
            var updateLabelDto = new UpdateLabelDto 
            { 
                Title = "Updated Label", 
                Color = "Red" 
            };
            OutputLabelDetailsDto? outputLabel = null;
            
            _mockLabelService
                .Setup(s => s.UpdateLabel(labelId, updateLabelDto, userId))
                .ReturnsAsync(outputLabel);

            var result = await _controller.UpdateLabel(labelId, updateLabelDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteLabel_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int userId = 1;
            int labelId = 1;
            
            _mockLabelService
                .Setup(s => s.DeleteLabel(labelId, userId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteLabel(labelId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteLabel_ReturnsNotFound_WhenDeletionFails()
        {
            int userId = 1;
            int labelId = 1;
            
            _mockLabelService
                .Setup(s => s.DeleteLabel(labelId, userId))
                .ReturnsAsync(false);

            var result = await _controller.DeleteLabel(labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
