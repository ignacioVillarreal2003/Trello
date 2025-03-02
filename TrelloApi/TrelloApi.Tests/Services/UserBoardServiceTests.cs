using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TrelloApi.Application.Services;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Services;

public class UserBoardServiceTests
{
    private readonly Mock<IUserBoardRepository> _mockUserBoardRepository;
    private readonly Mock<ILogger<UserBoardService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UserBoardService _service;

    public UserBoardServiceTests()
    {
        _mockUserBoardRepository = new Mock<IUserBoardRepository>();
        _mockLogger = new Mock<ILogger<UserBoardService>>();
        _mockMapper = new Mock<IMapper>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _service = new UserBoardService(
            _mockMapper.Object,
            _unitOfWork.Object,
            _mockUserBoardRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnsBoards_WhenBoardsFound()
    {
        const int boardId = 1;
        var users = new List<User>()
        {
            new User(email: "email1@gmail.com", username: "username 1", password: "password", theme: "theme"),
            new User(email: "email2@gmail.com", username: "username 2", password: "password", theme: "theme")
        };
        var response = new List<UserResponse>()
        {
            new UserResponse { Email = users[0].Email, Username = users[0].Username , Theme = users[0].Theme },
            new UserResponse { Email = users[1].Email, Username = users[1].Username , Theme = users[1].Theme }
        };

        _mockUserBoardRepository.Setup(r => r.GetUsersByBoardIdAsync(boardId)).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns(response);

        var result = await _service.GetUsersByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnsEmptyList_WhenBoardsFound()
    {
        const int boardId = 1;
        
        _mockUserBoardRepository.Setup(r => r.GetUsersByBoardIdAsync(boardId)).ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<UserResponse>>(It.IsAny<List<User>>())).Returns([]);

        var result = await _service.GetUsersByBoardId(boardId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddUsersToBoard_ShouldReturnsNull_WhenAddedSuccessful()
    {
        const int boardId = 1;
        var dto = new AddUserBoardDto { UserId = 1, Role = "Member" };
        var response = new UserBoardResponse { BoardId = boardId, UserId = dto.UserId, Role = dto.Role };
        
        _mockUserBoardRepository.Setup(r => r.CreateAsync(It.IsAny<UserBoard>()));
        _mockMapper.Setup(m => m.Map<UserBoardResponse>(It.IsAny<UserBoard>())).Returns(response);
        
        var result = await _service.AddUserToBoard(boardId, dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveUserFromBoard_ShouldReturnsTrue_WhenDeletedSuccessful()
    {
        const int boardId = 1, userId = 1;
        var userBoard = new UserBoard(userId: userId, boardId: boardId, role: "role");
        
        _mockUserBoardRepository.Setup(r => r
            .GetAsync(It.IsAny<Expression<Func<UserBoard, bool>>>())).ReturnsAsync(userBoard);
        _mockUserBoardRepository.Setup(r => r.DeleteAsync(It.IsAny<UserBoard>()));

        var result = await _service.RemoveUserFromBoard(boardId, userId);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveUserFromBoard_ShouldReturnsFalse_WhenDeletedUnsuccessful()
    {
        const int boardId = 1, userId = 1;
        var userBoard = new UserBoard(userId: userId, boardId: boardId, role: "role");
        
        _mockUserBoardRepository.Setup(r => r
            .GetAsync(It.IsAny<Expression<Func<UserBoard, bool>>>())).ReturnsAsync((UserBoard?)null);

        var result = await _service.RemoveUserFromBoard(boardId, userId);

        Assert.False(result);
    }
}