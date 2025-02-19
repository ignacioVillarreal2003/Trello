using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs; // OutputBoardDetailsDto, AddBoardDto, UpdateBoardDto, etc.
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Tests.Controllers
{
    public class BoardControllerTests
    {
        private readonly Mock<IBoardService> _mockBoardService;
        private readonly Mock<ILogger<BoardController>> _mockLogger;
        private readonly BoardController _controller;

        public BoardControllerTests()
        {
            _mockBoardService = new Mock<IBoardService>();
            _mockLogger = new Mock<ILogger<BoardController>>();

            _controller = new BoardController(_mockLogger.Object, _mockBoardService.Object);
            SetUserId(1);
        }

        private void SetUserId(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetBoardById_ReturnsOk_WithElementFound()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            var outputBoard = new OutputBoardDetailsDto
            {
                Id = boardId,
                Title = "board 1",
                Description = "",
                Background = "Blue",
                CreatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            _mockBoardService.Setup(s => s.GetBoardById(boardId, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.GetBoardById(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<OutputBoardDetailsDto>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(outputBoard.Id, value.Id);
            Assert.Equal(outputBoard.Title, value.Title);
            Assert.Equal(outputBoard.Description, value.Description);
            Assert.Equal(outputBoard.Background, value.Background);
            Assert.Equal(outputBoard.IsArchived, value.IsArchived);
        }

        [Fact]
        public async Task GetBoardById_ReturnsNotFound_WithElementNotFound()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            OutputBoardDetailsDto? outputBoard = null;

            _mockBoardService.Setup(s => s.GetBoardById(boardId, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.GetBoardById(boardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetBoardsByUserId_ReturnsOk_WithFullList()
        {
            // Arrange
            int userId = 1;
            var boards = new List<OutputBoardDetailsDto>
            {
                new OutputBoardDetailsDto { Id = 1, Title = "board 1", Description = "", Background = "Blue", CreatedAt = DateTime.UtcNow, IsArchived = false },
                new OutputBoardDetailsDto { Id = 2, Title = "board 2", Description = "", Background = "Red", CreatedAt = DateTime.UtcNow, IsArchived = false }
            };

            _mockBoardService.Setup(s => s.GetBoardsByUserId(userId))
                             .ReturnsAsync(boards);

            // Act
            var result = await _controller.GetBoardsByUserId();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<OutputBoardDetailsDto>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(2, value.Count);
        }

        [Fact]
        public async Task GetBoardsByUserId_ReturnsOk_WithEmptyList()
        {
            // Arrange
            int userId = 1;
            var boards = new List<OutputBoardDetailsDto>();

            _mockBoardService.Setup(s => s.GetBoardsByUserId(userId))
                             .ReturnsAsync(boards);

            // Act
            var result = await _controller.GetBoardsByUserId();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<OutputBoardDetailsDto>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }

        [Fact]
        public async Task GetBoardColors_ReturnsOk_WithColorsList()
        {
            // Arrange
            var backgrounds = new List<string> { "Blue", "Red", "Green" };

            // No mock es necesario si el método sólo retorna una lista estática
            // Act
            var result = await _controller.GetBoardColors();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<string>>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(backgrounds.Count, value.Count);
        }

        [Fact]
        public async Task AddBoard_ReturnsCreatedAtAction_WithElementCreated()
        {
            // Arrange
            int userId = 1;
            var addDto = new AddBoardDto { Title = "board 1", Background = "Blue", Description = "Test board" };
            var outputBoard = new OutputBoardDetailsDto
            {
                Id = 1,
                Title = "board 1",
                Description = "Test board",
                Background = "Blue",
                CreatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            _mockBoardService.Setup(s => s.AddBoard(addDto, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.AddBoard(addDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var value = Assert.IsAssignableFrom<OutputBoardDetailsDto>(createdResult.Value);

            // Assert
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(outputBoard.Id, value.Id);
            Assert.Equal(outputBoard.Title, value.Title);
            Assert.Equal(outputBoard.Background, value.Background);
            Assert.Equal(outputBoard.Description, value.Description);
            Assert.Equal(outputBoard.IsArchived, value.IsArchived);
        }

        [Fact]
        public async Task AddBoard_ReturnsBadRequest_WithElementNotCreated()
        {
            // Arrange
            int userId = 1;
            var addDto = new AddBoardDto { Title = "board 1", Background = "Blue" };
            OutputBoardDetailsDto? outputBoard = null;

            _mockBoardService.Setup(s => s.AddBoard(addDto, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.AddBoard(addDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            // Assert
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task UpdateBoard_ReturnsOk_WithElementUpdated()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            var updateDto = new UpdateBoardDto { Title = "board updated", Background = "Green", Description = "Updated description", IsArchived = false };
            var outputBoard = new OutputBoardDetailsDto
            {
                Id = boardId,
                Title = "board updated",
                Description = "Updated description",
                Background = "Green",
                CreatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            _mockBoardService.Setup(s => s.UpdateBoard(boardId, updateDto, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.UpdateBoard(boardId, updateDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<OutputBoardDetailsDto>(okResult.Value);

            // Assert
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(outputBoard.Id, value.Id);
            Assert.Equal(outputBoard.Title, value.Title);
        }

        [Fact]
        public async Task UpdateBoard_ReturnsNotFound_WithElementNotUpdated()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            var updateDto = new UpdateBoardDto { Title = "board updated" };
            OutputBoardDetailsDto? outputBoard = null;

            _mockBoardService.Setup(s => s.UpdateBoard(boardId, updateDto, userId))
                             .ReturnsAsync(outputBoard);

            // Act
            var result = await _controller.UpdateBoard(boardId, updateDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBoard_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            // En este caso, el servicio DeleteBoard retorna un booleano
            _mockBoardService.Setup(s => s.DeleteBoard(boardId, userId))
                             .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBoard(boardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);

            // Assert
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBoard_ReturnsNotFound_WhenElementNotDeleted()
        {
            // Arrange
            int userId = 1;
            int boardId = 1;
            _mockBoardService.Setup(s => s.DeleteBoard(boardId, userId))
                             .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBoard(boardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
