using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class UserBoardServiceTests
{
    private readonly Mock<IUserBoardRepository> _mockUserBoardRepository;
    private readonly Mock<ILogger<UserBoardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IBoardAuthorizationService> _mockBoardAuthorizationService;
    private readonly UserBoardService _service;

    public UserBoardServiceTests()
    {
        _mockUserBoardRepository = new Mock<IUserBoardRepository>();
        _mockLogger = new Mock<ILogger<UserBoardService>>();
        _mockMapper = new Mock<IMapper>();
        _mockBoardAuthorizationService = new Mock<IBoardAuthorizationService>();

        _service = new UserBoardService(_mockMapper.Object, _mockBoardAuthorizationService.Object, _mockLogger.Object, _mockUserBoardRepository.Object);
    }

    [Fact]
    public async Task GetUserBoardById_ReturnsOutputUserBoardDto_WhenUserBoardExists()
    {
        int boardId = 1, userId = 1;
        var userBoard = new UserBoard(userId: userId, boardId: boardId);
        var outputUserBoardDto = new AddUserBoardDto { UserId = userBoard.UserId, BoardId = userBoard.BoardId };

        _mockUserBoardRepository.Setup(r => r.GetUserBoardById(userId, boardId)).ReturnsAsync(userBoard);
        _mockMapper.Setup(m => m.Map<AddUserBoardDto>(userBoard)).Returns(outputUserBoardDto);

        var result = await _service.GetUserBoardById(userId, boardId);

        Assert.NotNull(result);
        Assert.Equal(outputUserBoardDto.UserId, result.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, result.BoardId);
    }

    [Fact]
    public async Task GetUserBoardById_ReturnsNull_WhenUserBoardDoesNotExist()
    {
        int boardId = 1, userId = 1;

        _mockUserBoardRepository.Setup(r => r.GetUserBoardById(userId, boardId)).ReturnsAsync((UserBoard?)null);

        var result = await _service.GetUserBoardById(userId, boardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddUserBoard_ReturnsOutputUserBoardDto_WhenUserBoardIsAdded()
    {
        int boardId = 1, userId = 1;
        var addUserBoardDto = new AddUserBoardDto { UserId = userId, BoardId = boardId };
        var newUserBoard = new UserBoard(userId: addUserBoardDto.UserId, boardId: addUserBoardDto.BoardId);
        var outputUserBoardDto = new AddUserBoardDto { UserId = newUserBoard.UserId, BoardId = newUserBoard.BoardId };

        _mockUserBoardRepository.Setup(r => r.AddUserBoard(It.IsAny<UserBoard>())).ReturnsAsync(newUserBoard);
        _mockMapper.Setup(m => m.Map<AddUserBoardDto>(newUserBoard)).Returns(outputUserBoardDto);

        var result = await _service.AddUserBoard(addUserBoardDto, userId);

        Assert.NotNull(result);
        Assert.Equal(outputUserBoardDto.UserId, result.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, result.BoardId);
    }

    [Fact]
    public async Task AddUserBoard_ReturnsNull_WhenRepositoryReturnsNull()
    {
        int boardId = 1, userId = 1;
        var addUserBoardDto = new AddUserBoardDto { UserId = userId, BoardId = boardId };

        _mockUserBoardRepository.Setup(r => r.AddUserBoard(It.IsAny<UserBoard>())).ReturnsAsync((UserBoard?)null);

        var result = await _service.AddUserBoard(addUserBoardDto, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserBoard_ReturnsOutputUserBoardDto_WhenDeletionIsSuccessful()
    {
        int boardId = 1, userId = 1, userToDeleteId = 2;
        var existingUserBoard = new UserBoard(userId: userToDeleteId, boardId: boardId);
        var deletedUserBoard = existingUserBoard;
        var outputUserBoardDto = new AddUserBoardDto { UserId = deletedUserBoard.UserId, BoardId = deletedUserBoard.BoardId };

        _mockUserBoardRepository.Setup(r => r.GetUserBoardById(userToDeleteId, boardId)).ReturnsAsync(existingUserBoard);
        _mockUserBoardRepository.Setup(r => r.DeleteUserBoard(existingUserBoard)).ReturnsAsync(deletedUserBoard);
        _mockMapper.Setup(m => m.Map<AddUserBoardDto>(deletedUserBoard)).Returns(outputUserBoardDto);

        var result = await _service.DeleteUserBoard(userToDeleteId, boardId, userId);

        Assert.NotNull(result);
        Assert.Equal(outputUserBoardDto.UserId, result.UserId);
        Assert.Equal(outputUserBoardDto.BoardId, result.BoardId);
    }

    [Fact]
    public async Task DeleteUserBoard_ReturnsNull_WhenUserBoardNotFound()
    {
        int boardId = 1, userId = 1, userToDeleteId = 2;

        _mockUserBoardRepository.Setup(r => r.GetUserBoardById(userToDeleteId, boardId)).ReturnsAsync((UserBoard?)null);

        var result = await _service.DeleteUserBoard(userToDeleteId, userId, boardId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserBoard_ReturnsNull_WhenDeletionFails()
    {
        int boardId = 1, userId = 1, userToDeleteId = 2;
        var existingUserBoard = new UserBoard(userId: userToDeleteId, boardId: boardId);

        _mockUserBoardRepository.Setup(r => r.GetUserBoardById(userToDeleteId, boardId)).ReturnsAsync(existingUserBoard);
        _mockUserBoardRepository.Setup(r => r.DeleteUserBoard(existingUserBoard)).ReturnsAsync((UserBoard?)null);

        var result = await _service.DeleteUserBoard(userToDeleteId, boardId, userId);

        Assert.Null(result);
    }
}