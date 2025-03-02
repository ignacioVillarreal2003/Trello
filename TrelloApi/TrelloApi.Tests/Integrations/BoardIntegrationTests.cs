using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class BoardIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public BoardIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetBoardById_ShouldReturnsBoard_WhenBoardFound()
    {
        var board = new Board(title: "title", background: "background");
        
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/Board/{board.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBoardById_ShouldReturnsNotFound_WhenBoardNotFound()
    {
        const int boardId = 1;
        
        var response = await _client.GetAsync($"/Board/{boardId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnsBoards_WhenBoardsFound()
    {
        var board1 = new Board(title: "title 1", background: "background") { Id = 1 };
        var board2 = new Board(title: "title 2", background: "background") { Id = 2 };
        var user = new User(email: "user@email.com", username: "user", password: "password", theme: "theme") { Id = 1 };
        var userBoard1 = new UserBoard(user.Id, board1.Id);
        var userBoard2 = new UserBoard(user.Id, board2.Id);
        
        _dbContext.Boards.Add(board1);
        _dbContext.Boards.Add(board2);
        _dbContext.Users.Add(user);
        _dbContext.UserBoards.Add(userBoard1);
        _dbContext.UserBoards.Add(userBoard2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Board");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<BoardResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Equal(2, boards.Count);
    }
    
    [Fact]
    public async Task GetBoardsByUserId_ShouldReturnsEmptyList_WhenBoardsNotFound()
    {
        var response = await _client.GetAsync($"/Board");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<BoardResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Empty(boards);
    }
    
    [Fact]
    public async Task GetBoardColors_ShouldReturnsOk_WhenColorsFound()
    {
        var response = await _client.GetAsync($"/Board/colors");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var colors = JsonSerializer.Deserialize<List<string>>(body);
        
        Assert.NotNull(colors);
        Assert.NotEmpty(colors);
    }
    
    [Fact]
    public async Task AddBoard_ShouldReturnsCreated_WhenAddedSuccessful()
    {
        var dto = new AddBoardDto { Title = "title", Background = "background-1.svg" };
        
        var response = await _client.PostAsJsonAsync("/Board", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBoard_ShouldReturnsOk_WhenUpdatedSuccessful()
    {
        var board = new Board(title: "title", background: "background");
        var dto = new UpdateBoardDto { Title = "updated title" };

        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PutAsJsonAsync($"/Board/{board.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBoard_ShouldReturnsBadRequest_WhenUpdatedUnsuccessful()
    {
        const int boardId = 1;
        var dto = new UpdateBoardDto { Title = "updated title" };
        
        var response = await _client.PutAsJsonAsync($"/Board/{boardId}", dto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBoard_ShouldReturnsNoContent_WhenDeletedSuccessful()
    {
        var board = new Board(title: "title", background: "background");
        
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/Board/{board.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBoard_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
    {
        const int boardId = 1;

        var response = await _client.DeleteAsync($"/Board/{boardId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}