using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Controllers;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Interfaces.Services;
using Xunit;

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
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        
        [Fact]
        public async Task GetListById_ReturnsOk_WhenListFound()
        {
            int userId = 1;
            int listId = 1;
            var outputList = new OutputListDetailsDto
            {
                Id = listId,
                Title = "List 1",
                Position = 1,
                BoardId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockListService.Setup(s => s.GetListById(listId, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.GetListById(listId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedList = Assert.IsType<OutputListDetailsDto>(okResult.Value);
            Assert.Equal(outputList.Id, returnedList.Id);
            Assert.Equal(outputList.Title, returnedList.Title);
            Assert.Equal(outputList.Position, returnedList.Position);
            Assert.Equal(outputList.BoardId, returnedList.BoardId);
        }
        
        [Fact]
        public async Task GetListById_ReturnsNotFound_WhenListNotFound()
        {
            int userId = 1;
            int listId = 1;
            OutputListDetailsDto? outputList = null;
            
            _mockListService.Setup(s => s.GetListById(listId, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.GetListById(listId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task GetListsByBoardId_ReturnsOk_WithLists()
        {
            int userId = 1;
            int boardId = 1;
            var lists = new List<OutputListDetailsDto>
            {
                new OutputListDetailsDto { Id = 1, Title = "List 1", Position = 1, BoardId = boardId, CreatedAt = DateTime.UtcNow },
                new OutputListDetailsDto { Id = 2, Title = "List 2", Position = 2, BoardId = boardId, CreatedAt = DateTime.UtcNow }
            };

            _mockListService.Setup(s => s.GetListsByBoardId(boardId, userId))
                            .ReturnsAsync(lists);

            var result = await _controller.GetListsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLists = Assert.IsType<List<OutputListDetailsDto>>(okResult.Value);
            Assert.Equal(lists.Count, returnedLists.Count);
        }
        
        [Fact]
        public async Task GetListsByBoardId_ReturnsOk_WithEmptyList()
        {
            int userId = 1;
            int boardId = 1;
            var lists = new List<OutputListDetailsDto>();

            _mockListService.Setup(s => s.GetListsByBoardId(boardId, userId))
                            .ReturnsAsync(lists);

            var result = await _controller.GetListsByBoardId(boardId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedLists = Assert.IsType<List<OutputListDetailsDto>>(okResult.Value);
            Assert.Empty(returnedLists);
        }
        
        [Fact]
        public async Task AddList_ReturnsCreated_WhenListIsAdded()
        {
            int userId = 1;
            int boardId = 1;
            var addListDto = new AddListDto { Title = "List 1", Position = 1 };
            var outputList = new OutputListDetailsDto
            {
                Id = 1,
                Title = "List 1",
                Position = 1,
                BoardId = boardId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _mockListService.Setup(s => s.AddList(boardId, addListDto, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.AddList(boardId, addListDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            var returnedList = Assert.IsType<OutputListDetailsDto>(createdResult.Value);
            Assert.Equal(outputList.Id, returnedList.Id);
            Assert.Equal(outputList.Title, returnedList.Title);
            Assert.Equal(outputList.Position, returnedList.Position);
            Assert.Equal(outputList.BoardId, returnedList.BoardId);
        }
        
        [Fact]
        public async Task AddList_ReturnsBadRequest_WhenListNotAdded()
        {
            int userId = 1;
            int boardId = 1;
            var addListDto = new AddListDto { Title = "List 1", Position = 1 };
            OutputListDetailsDto? outputList = null;
            
            _mockListService.Setup(s => s.AddList(boardId, addListDto, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.AddList(boardId, addListDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateList_ReturnsOk_WhenListIsUpdated()
        {
            int userId = 1;
            int listId = 1;
            var updateListDto = new UpdateListDto { Title = "Updated List", Position = 2 };
            var outputList = new OutputListDetailsDto
            {
                Id = listId,
                Title = "Updated List",
                Position = 2,
                BoardId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockListService.Setup(s => s.UpdateList(listId, updateListDto, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.UpdateList(listId, updateListDto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedList = Assert.IsType<OutputListDetailsDto>(okResult.Value);
            Assert.Equal(outputList.Id, returnedList.Id);
            Assert.Equal(outputList.Title, returnedList.Title);
            Assert.Equal(outputList.Position, returnedList.Position);
            Assert.Equal(outputList.BoardId, returnedList.BoardId);
        }
        
        [Fact]
        public async Task UpdateList_ReturnsNotFound_WhenListNotFound()
        {
            int userId = 1;
            int listId = 1;
            var updateListDto = new UpdateListDto { Title = "Updated List", Position = 2 };
            OutputListDetailsDto? outputList = null;
            
            _mockListService.Setup(s => s.UpdateList(listId, updateListDto, userId))
                            .ReturnsAsync(outputList);

            var result = await _controller.UpdateList(listId, updateListDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteList_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            int userId = 1;
            int listId = 1;
            
            _mockListService.Setup(s => s.DeleteList(listId, userId))
                            .ReturnsAsync(true);

            var result = await _controller.DeleteList(listId);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task DeleteList_ReturnsNotFound_WhenDeletionFails()
        {
            int userId = 1;
            int listId = 1;
            
            _mockListService.Setup(s => s.DeleteList(listId, userId))
                            .ReturnsAsync(false);

            var result = await _controller.DeleteList(listId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
