using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.List;

namespace TrelloApi.Tests.Controllers
{
    public class ListControllerTests
    {
        private readonly Mock<IListService> _mockListService;
        private readonly Mock<ILogger<ListController>> _mockLogger;
        private readonly ListController _controller;

        public ListControllerTests()
        {
            _mockListService = new Mock<IListService>();
            _mockLogger = new Mock<ILogger<ListController>>();
            _controller = new ListController(_mockLogger.Object, _mockListService.Object);
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
        public async Task GetListById_ShouldReturnsOk_WhenListFound()
        {
            const int listId = 1;
            var response = new ListResponse
            {
                Id = listId,
                Title = "title",
                Position = 1,
                BoardId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockListService.Setup(s => s.GetListById(listId)).ReturnsAsync(response);

            var result = await _controller.GetListById(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task GetListById_ShouldReturnsNotFound_WhenListNotFound()
        {
            const int listId = 1;
            
            _mockListService.Setup(s => s.GetListById(listId)).ReturnsAsync((ListResponse?)null);

            var result = await _controller.GetListById(listId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetListsByBoardId_ShouldReturnsOk_WhenListsFound()
        {
            const int boardId = 1;
            var response = new List<ListResponse>
            {
                new ListResponse { Id = 1, Title = "title 1", Position = 1, BoardId = boardId, CreatedAt = DateTime.UtcNow },
                new ListResponse { Id = 2, Title = "title 2", Position = 2, BoardId = boardId, CreatedAt = DateTime.UtcNow }
            };

            _mockListService.Setup(s => s.GetListsByBoardId(boardId)).ReturnsAsync(response);

            var result = await _controller.GetListsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<ListResponse>>(okResult.Value);
            
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(response.Count, value.Count);
        }
        
        [Fact]
        public async Task GetListsByBoardId_ShouldReturnsOk_WhenListsNotFound()
        {
            const int boardId = 1;

            _mockListService.Setup(s => s.GetListsByBoardId(boardId)).ReturnsAsync([]);

            var result = await _controller.GetListsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<List<ListResponse>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(value);
        }
        
        [Fact]
        public async Task AddList_ShouldReturnsCreated_WhenAddedSuccessful()
        {
            const int boardId = 1;
            var dto = new AddListDto { Title = "title", Position = 1 };
            var response = new ListResponse
            {
                Id = 1,
                Title = dto.Title,
                Position = dto.Position,
                BoardId = boardId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockListService.Setup(s => s.AddList(boardId, dto)).ReturnsAsync(response);

            var result = await _controller.AddList(boardId, dto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            
            Assert.Equal(201, createdResult.StatusCode);
        }
        
        [Fact]
        public async Task AddList_ShouldReturnsBadRequest_WhenAddedUnsuccessful()
        {
            const int boardId = 1;
            var dto = new AddListDto { Title = "title", Position = 1 };
            
            _mockListService.Setup(s => s.AddList(boardId, dto)).ReturnsAsync((ListResponse?)null);

            var result = await _controller.AddList(boardId, dto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateList_ShouldReturnsOk_WhenUpdatedSuccessful()
        {
            const int listId = 1;
            var dto = new UpdateListDto { Title = "updated title" };
            var response = new ListResponse
            {
                Id = listId,
                Title = dto.Title,
                Position = 1,
                BoardId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockListService.Setup(s => s.UpdateList(listId, dto)).ReturnsAsync(response);

            var result = await _controller.UpdateList(listId, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateList_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
        {
            const int listId = 1;
            var dto = new UpdateListDto { Title = "updated title" };
            
            _mockListService.Setup(s => s.UpdateList(listId, dto)).ReturnsAsync((ListResponse?)null);

            var result = await _controller.UpdateList(listId, dto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteList_ShouldReturnsNoContent_WhenDeletedSuccessful()
        {
            const int listId = 1;
            
            _mockListService.Setup(s => s.DeleteList(listId)).ReturnsAsync(true);

            var result = await _controller.DeleteList(listId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteList_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
        {
            const int listId = 1;
            
            _mockListService.Setup(s => s.DeleteList(listId)).ReturnsAsync(false);

            var result = await _controller.DeleteList(listId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
