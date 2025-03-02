using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserBoard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class UserBoardIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public UserBoardIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnUsers_WhenUsersFound()
    {
        var board = new Board("title", "background") { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var userBoard = new UserBoard(user.Id, board.Id);
        
        _dbContext.Boards.Add(board);
        _dbContext.Users.Add(user);
        _dbContext.UserBoards.Add(userBoard);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/UserBoard/board/{board.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);

        Assert.NotNull(users);
        Assert.Single(users);
    }

    [Fact]
    public async Task GetUsersByBoardId_ShouldReturnEmptyList_WhenUsersNotFound()
    {
        const int boardId = 1;
        
        var response = await _client.GetAsync($"/UserBoard/board/{boardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task AddUserToBoard_ShouldReturnCreated_WhenAddedSuccessful()
    {
        var board = new Board("title", "background") { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var dto = new AddUserBoardDto { UserId = user.Id, Role = "Member" };

        _dbContext.Boards.Add(board);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync($"/UserBoard/board/{board.Id}", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromBoard_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var board = new Board("title", "background") { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var userBoard = new UserBoard(user.Id, board.Id);

        _dbContext.Boards.Add(board);
        _dbContext.Users.Add(user);
        _dbContext.UserBoards.Add(userBoard);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/UserBoard/board/{board.Id}/user/{user.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromBoard_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int boardId = 1, userId = 1;
        
        var response = await _client.DeleteAsync($"/UserBoard/board/{boardId}/user/{userId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
