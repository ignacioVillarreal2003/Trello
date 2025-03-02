using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Label;

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
        public async Task GetLabelById_ShouldReturnsOk_WhenLabelFound()
        {
            const int labelId = 1;
            var response = new LabelResponse 
            { 
                Id = labelId, 
                Title = "title", 
                Color = "color", 
                BoardId = 1 
            };

            _mockLabelService.Setup(s => s.GetLabelById(labelId)).ReturnsAsync(response);

            var result = await _controller.GetLabelById(labelId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task GetLabelById_ShouldReturnsNotFound_WhenLabelNotFound()
        {
            const int labelId = 1;
            
            _mockLabelService.Setup(s => s.GetLabelById(labelId)).ReturnsAsync((LabelResponse?)null);

            var result = await _controller.GetLabelById(labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetLabelsByBoardId_ShouldReturnsOk_WhenLabelsFound()
        {
            const int boardId = 1;
            var response = new List<LabelResponse>
            {
                new LabelResponse { Id = 1, Title = "title 1", Color = "color", BoardId = boardId },
                new LabelResponse { Id = 2, Title = "title 2", Color = "color", BoardId = boardId }
            };

            _mockLabelService.Setup(s => s.GetLabelsByBoardId(boardId)).ReturnsAsync(response);

            var result = await _controller.GetLabelsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<LabelResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetLabelsByBoardId_ShouldReturnsOk_WhenLabelsNotFound()
        {
            const int boardId = 1;

            _mockLabelService.Setup(s => s.GetLabelsByBoardId(boardId)).ReturnsAsync([]);

            var result = await _controller.GetLabelsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<LabelResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task GetLabelColors_ShouldReturnsOk_WithLabelColorsFound()
        {
            var result = await _controller.GetLabelColors();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedColors = Assert.IsType<List<string>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotEmpty(returnedColors);
        }
        
        [Fact]
        public async Task AddLabel_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int boardId = 1;
            var dto = new AddLabelDto 
            { 
                Title = "title", 
                Color = "color"
            };
            var response = new LabelResponse 
            { 
                Id = 1, 
                Title = dto.Title, 
                Color = dto.Color
            };

            _mockLabelService.Setup(s => s.AddLabel(boardId, dto)).ReturnsAsync(response);

            var result = await _controller.AddLabel(boardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddLabel_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int boardId = 1;
            var dto = new AddLabelDto 
            { 
                Title = "title", 
                Color = "color"
            };
            
            _mockLabelService.Setup(s => s.AddLabel(boardId, dto)).ReturnsAsync((LabelResponse?)null);

            var result = await _controller.AddLabel(boardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateLabel_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int labelId = 1;
            var dto = new UpdateLabelDto { Title = "updated title", };
            var response = new LabelResponse 
            { 
                Id = labelId, 
                Title = dto.Title, 
                Color = "color", 
                BoardId = 1 
            };

            _mockLabelService.Setup(s => s.UpdateLabel(labelId, dto)).ReturnsAsync(response);

            var result = await _controller.UpdateLabel(labelId, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateLabel_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int labelId = 1;
            var dto = new UpdateLabelDto { Title = "updated title", };
            
            _mockLabelService.Setup(s => s.UpdateLabel(labelId, dto)).ReturnsAsync((LabelResponse?)null);

            var result = await _controller.UpdateLabel(labelId, dto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteLabel_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int labelId = 1;
            
            _mockLabelService.Setup(s => s.DeleteLabel(labelId)).ReturnsAsync(true);

            var result = await _controller.DeleteLabel(labelId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteLabel_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int labelId = 1;
            
            _mockLabelService.Setup(s => s.DeleteLabel(labelId)).ReturnsAsync(false);

            var result = await _controller.DeleteLabel(labelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
