using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.DTOs; // Aquí se encuentran los DTOs: AddListDto, UpdateListDto, OutputListDetailsDto

namespace TrelloApi.Tests.Services
{
    public class ListServiceTests
    {
        private readonly Mock<IListRepository> _mockListRepository;
        private readonly Mock<ILogger<ListService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
        private readonly ListService _service;

        public ListServiceTests()
        {
            _mockListRepository = new Mock<IListRepository>();
            _mockLogger = new Mock<ILogger<ListService>>();
            _mockMapper = new Mock<IMapper>();
            _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

            _service = new ListService(
                _mockMapper.Object, 
                _mockBoardAuthorizationService.Object, 
                _mockLogger.Object, 
                _mockListRepository.Object);
        }
        
        [Fact]
        public async Task GetListById_ReturnsOutputListDetailsDto_WhenListExists()
        {
            // Arrange
            int listId = 1, userId = 1;
            var list = new List(title: "List 1", boardId: 1, position: 0) { Id = listId };
            var outputDto = new OutputListDetailsDto 
            { 
                Id = list.Id, 
                Title = list.Title, 
                Position = list.Position 
            };

            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync(list);
            _mockMapper.Setup(m => m.Map<OutputListDetailsDto>(list))
                       .Returns(outputDto);

            // Act
            var result = await _service.GetListById(listId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Position, result.Position);
        }

        [Fact]
        public async Task GetListById_ReturnsNull_WhenListDoesNotExist()
        {
            // Arrange
            int listId = 1, userId = 1;
            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.GetListById(listId, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetListsByBoardId_ReturnsListOfOutputListDetailsDto_WhenListsExist()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var lists = new List<List>
            {
                new List(title: "List 1", boardId: boardId, position: 0) { Id = 1 },
                new List(title: "List 2", boardId: boardId, position: 1) { Id = 2 }
            };
            var outputDtos = new List<OutputListDetailsDto>
            {
                new OutputListDetailsDto { Id = lists[0].Id, Title = lists[0].Title, Position = lists[0].Position },
                new OutputListDetailsDto { Id = lists[1].Id, Title = lists[1].Title, Position = lists[1].Position }
            };

            _mockListRepository.Setup(r => r.GetListsByBoardId(boardId))
                               .ReturnsAsync(lists);
            _mockMapper.Setup(m => m.Map<List<OutputListDetailsDto>>(lists))
                       .Returns(outputDtos);

            // Act
            var result = await _service.GetListsByBoardId(boardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(outputDtos[0].Id, result[0].Id);
            Assert.Equal(outputDtos[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetListsByBoardId_ReturnsEmptyList_WhenNoListsExist()
        {
            // Arrange
            int boardId = 1, userId = 1;
            _mockListRepository.Setup(r => r.GetListsByBoardId(boardId))
                               .ReturnsAsync(new List<List>());
            _mockMapper.Setup(m => m.Map<List<OutputListDetailsDto>>(It.IsAny<List<List>>()))
                       .Returns(new List<OutputListDetailsDto>());

            // Act
            var result = await _service.GetListsByBoardId(boardId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task AddList_ReturnsOutputListDetailsDto_WhenListIsAdded()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addListDto = new AddListDto { Title = "New List", Position = 0 };
            var newList = new List(title: addListDto.Title, boardId: boardId, position: addListDto.Position) { Id = 1 };
            var outputDto = new OutputListDetailsDto 
            { 
                Id = newList.Id, 
                Title = newList.Title, 
                Position = newList.Position 
            };

            _mockListRepository.Setup(r => r.AddList(It.IsAny<List>()))
                               .ReturnsAsync(newList);
            _mockMapper.Setup(m => m.Map<OutputListDetailsDto>(newList))
                       .Returns(outputDto);

            // Act
            var result = await _service.AddList(boardId, addListDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Position, result.Position);
        }

        [Fact]
        public async Task AddList_ReturnsNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            int boardId = 1, userId = 1;
            var addListDto = new AddListDto { Title = "New List", Position = 0 };

            _mockListRepository.Setup(r => r.AddList(It.IsAny<List>()))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.AddList(boardId, addListDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task UpdateList_ReturnsOutputListDetailsDto_WhenUpdateIsSuccessful()
        {
            // Arrange
            int listId = 1, userId = 1;
            var existingList = new List(title: "Old Title", boardId: 1, position: 0) { Id = listId };
            var updateListDto = new UpdateListDto { Title = "New Title", Position = 1 };
            var updatedList = new List(title: updateListDto.Title, boardId: existingList.BoardId, position: updateListDto.Position.Value) { Id = existingList.Id };
            // Se asume que UpdatedAt se actualiza en la entidad, pero no se mapea en el DTO
            var outputDto = new OutputListDetailsDto 
            { 
                Id = updatedList.Id, 
                Title = updatedList.Title, 
                Position = updatedList.Position 
            };

            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync(existingList);
            _mockListRepository.Setup(r => r.UpdateList(existingList))
                               .ReturnsAsync(updatedList);
            _mockMapper.Setup(m => m.Map<OutputListDetailsDto>(updatedList))
                       .Returns(outputDto);

            // Act
            var result = await _service.UpdateList(listId, updateListDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(outputDto.Id, result.Id);
            Assert.Equal(outputDto.Title, result.Title);
            Assert.Equal(outputDto.Position, result.Position);
        }

        [Fact]
        public async Task UpdateList_ReturnsNull_WhenListNotFound()
        {
            // Arrange
            int listId = 1, userId = 1;
            var updateListDto = new UpdateListDto { Title = "New Title" };

            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.UpdateList(listId, updateListDto, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateList_ReturnsNull_WhenUpdateFails()
        {
            // Arrange
            int listId = 1, userId = 1;
            var existingList = new List(title: "Old Title", boardId: 1, position: 0) { Id = listId };
            var updateListDto = new UpdateListDto { Title = "New Title" };

            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync(existingList);
            _mockListRepository.Setup(r => r.UpdateList(existingList))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.UpdateList(listId, updateListDto, userId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task DeleteList_ReturnsTrue_WhenDeletionIsSuccessful()
        {
            // Arrange
            int listId = 1, userId = 1;
            var existingList = new List(title: "List", boardId: 1, position: 0) { Id = listId };

            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync(existingList);
            // En la nueva implementación, DeleteList retorna un booleano
            _mockListRepository.Setup(r => r.DeleteList(existingList))
                               .ReturnsAsync(It.IsAny<List>());

            // Act
            var result = await _service.DeleteList(listId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteList_ReturnsFalse_WhenListNotFound()
        {
            // Arrange
            int listId = 1, userId = 1;
            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.DeleteList(listId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteList_ReturnsFalse_WhenDeletionFails()
        {
            // Arrange
            int listId = 1, userId = 1;
            var existingList = new List(title: "List 1", boardId: 1, position: 0) { Id = listId };
            
            _mockListRepository.Setup(r => r.GetListById(listId))
                               .ReturnsAsync(existingList);
            _mockListRepository.Setup(r => r.DeleteList(existingList))
                               .ReturnsAsync((List?)null);

            // Act
            var result = await _service.DeleteList(listId, userId);

            // Assert
            Assert.False(result);
        }
    }
}
