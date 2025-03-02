using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Board;

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
        public async Task GetBoardById_ShouldReturnsBoard_WhenBoardNotFound()
        {
            int boardId = 1;
            var response = new BoardResponse 
            {
                Id = boardId,
                Title = "title",
                Description = "description",
                Background = "background",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsArchived = false,
                ArchivedAt = null
            };

            _mockBoardService.Setup(s => s.GetBoardById(boardId)).ReturnsAsync(response);

            var result = await _controller.GetBoardById(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<BoardResponse>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Id, value.Id);
        }

        [Fact]
        public async Task GetBoardById_ShouldReturnsNotFound_WhenBoardNotFound()
        {
            const int boardId = 1;

            _mockBoardService.Setup(s => s.GetBoardById(boardId)).ReturnsAsync((BoardResponse?)null);

            var result = await _controller.GetBoardById(boardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetBoardsByUserId_ShouldReturnsOk_WhenBoardsFound()
        {
            const int userId = 1;
            var response = new List<BoardResponse>
            {
                new BoardResponse { Id = 1, Title = "title 1", Description = "description 1", Background = "background", CreatedAt = DateTime.UtcNow, UpdatedAt = null, IsArchived = false, ArchivedAt = null },
                new BoardResponse { Id = 2, Title = "title 2", Description = "description 2", Background = "background", CreatedAt = DateTime.UtcNow, UpdatedAt = null, IsArchived = false, ArchivedAt = null }
            };

            _mockBoardService.Setup(s => s.GetBoardsByUserId(userId)).ReturnsAsync(response);

            var result = await _controller.GetBoardsByUserId();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<BoardResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }

        [Fact]
        public async Task GetBoardsByUserId_ShouldReturnOk_WhenBoardsNotFound()
        {
            const int userId = 1;
            
            _mockBoardService.Setup(s => s.GetBoardsByUserId(userId)).ReturnsAsync([]);

            var result = await _controller.GetBoardsByUserId();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<BoardResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }

        [Fact]
        public async Task GetBoardColors_ShouldReturnsOk_WhenColorsFound()
        {
            var result = await _controller.GetBoardColors();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<List<string>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task AddBoard_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int userId = 1;
            var dto = new AddBoardDto { Title = "title", Background = "background", Description = "description" };
            var response = new BoardResponse
            {
                Id = 1,
                Title = dto.Title,
                Description = dto.Description,
                Background = dto.Background,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsArchived = false,
                ArchivedAt = null
            };

            _mockBoardService.Setup(s => s.AddBoard(dto, userId)).ReturnsAsync(response);

            var result = await _controller.AddBoard(dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task AddBoard_ReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int userId = 1;
            var dto = new AddBoardDto { Title = "title", Description = "description", Background = "background" };

            _mockBoardService.Setup(s => s.AddBoard(dto, userId)).ReturnsAsync((BoardResponse?)null);

            var result = await _controller.AddBoard(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task UpdateBoard_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int boardId = 1;
            var dto = new UpdateBoardDto { Title = "updated title" };
            var response = new BoardResponse
            {
                Id = boardId,
                Title = "title",
                Description = "description",
                Background = "background",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsArchived = false,
                ArchivedAt = null
            };

            _mockBoardService.Setup(s => s.UpdateBoard(boardId, dto)).ReturnsAsync(response);

            var result = await _controller.UpdateBoard(boardId, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateBoard_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int boardId = 1;
            var updateDto = new UpdateBoardDto { Title = "updated title" };

            _mockBoardService.Setup(s => s.UpdateBoard(boardId, updateDto)).ReturnsAsync((BoardResponse?)null);

            var result = await _controller.UpdateBoard(boardId, updateDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBoard_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int boardId = 1;

            _mockBoardService.Setup(s => s.DeleteBoard(boardId)).ReturnsAsync(true);

            var result = await _controller.DeleteBoard(boardId);
            var noContentResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBoard_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int boardId = 1;
            
            _mockBoardService.Setup(s => s.DeleteBoard(boardId)).ReturnsAsync(false);

            var result = await _controller.DeleteBoard(boardId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
