using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.app;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Authentication;

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

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwt>();
        var token = jwtService.GenerateToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetBoardById_ReturnsOk_WhenBoardExists()
    {
        var board = new Board(title: "title", icon: "icon", theme: "theme");
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Board/{board.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBoardById_ReturnsNotFound_WhenBoardNotExists()
    {
        var boardId = 1;
        var response = await _client.GetAsync($"/Board/{boardId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBoards_ReturnsOk_WithBoardList()
    {
        var board1 = new Board(title: "title", icon: "icon", theme: "theme");
        var board2 = new Board(title: "title", icon: "icon", theme: "theme");
        _dbContext.Boards.Add(board1);
        _dbContext.Boards.Add(board2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Board");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetBoards_ReturnsOk_WithEmptyBoardList()
    {
        var response = await _client.GetAsync($"/Board");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task AddBoard_ReturnsCreated_WhenBoardIsAdded()
    {
        var addBoardDto = new { Title = "Test Board", Theme = "Blue", Icon = "TestIcon" };
        
        var response = await _client.PostAsJsonAsync("/Board", addBoardDto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task AddBoard_ReturnsNotCreated_WhenBoardIsNotAdded()
    {
        var addBoardDto = new { Title = "Test Board" };
        
        var response = await _client.PostAsJsonAsync("/Board", addBoardDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBoard_ReturnsOk_WhenBoardIsUpdated()
    {
        var board = new Board(title: "title", icon: "icon", theme: "theme");
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();
        
        var updateBoardDto = new { title = "Updated Board" };
        
        var response = await _client.PutAsJsonAsync($"/Board/{board.Id}", updateBoardDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateBoard_ReturnsOk_WhenBoardIsNotUpdated()
    {
        var boardId = 1;
        
        var updateBoardDto = new { title = "Updated Board" };
        
        var response = await _client.PutAsJsonAsync($"/Board/{boardId}", updateBoardDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBoard_ReturnsOk_WhenBoardIsDeleted()
    {
        var board = new Board(title: "title", icon: "icon", theme: "theme");
        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.DeleteAsync($"/Board/{board.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBoard_ReturnsNotFound_WhenBoardIsNotDeleted()
    {
        var boardId = 1;
        
        var response = await _client.DeleteAsync($"/Board/{boardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}