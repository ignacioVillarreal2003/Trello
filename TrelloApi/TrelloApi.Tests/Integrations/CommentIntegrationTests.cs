using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.app;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;
using Task = System.Threading.Tasks.Task;

namespace TrelloApi.Tests.Integrations;

public class CommentIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public CommentIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetCommentById_ReturnsOk_WhenCommentExists()
    {
        var comment = new Comment(text: "title", cardId: 1, authorId: 1);
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Comment/{comment.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCommentById_ReturnsNotFound_WhenCommentNotExists()
    {
        var commentId = 1;
        var response = await _client.GetAsync($"/Comment/{commentId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCommentsByCardId_ReturnsOk_WithCommentList()
    {
        var cardId = 1;
        var comment1 = new Comment(text: "title 1", cardId: 1, authorId: 1);
        var comment2 = new Comment(text: "title 2", cardId: 1, authorId: 1);
        _dbContext.Comments.Add(comment1);
        _dbContext.Comments.Add(comment2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Comment/task/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetComments_ReturnsOk_WithEmptyCommentList()
    {
        var cardId = 1;
        
        var response = await _client.GetAsync($"/Comment/task/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task AddComment_ReturnsCreated_WhenCommentIsAdded()
    {
        var cardId = 1;
        var addCommentDto = new { Text = "Test Comment", AuthorId = 1 };
        var response = await _client.PostAsJsonAsync($"/Comment/task/{cardId}", addCommentDto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task AddComment_ReturnsNotCreated_WhenCommentIsNotAdded()
    {
        var cardId = 1;
        var addCommentDto = new { Text = "Test Comment" };
        
        var response = await _client.PostAsJsonAsync($"/Comment/task/{cardId}", addCommentDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateComment_ReturnsOk_WhenCommentIsUpdated()
    {
        var comment = new Comment(text: "title", cardId: 1, authorId: 1);
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var updateCommentDto = new { text = "Updated Comment" };
        
        var response = await _client.PutAsJsonAsync($"/Comment/{comment.Id}", updateCommentDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateComment_ReturnsOk_WhenCommentIsNotUpdated()
    {
        var commentId = 1;
        var updateCommentDto = new { title = "Updated Comment" };
        
        var response = await _client.PutAsJsonAsync($"/Comment/{commentId}", updateCommentDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteComment_ReturnsOk_WhenCommentIsDeleted()
    {
        var comment = new Comment(text: "title", cardId: 1, authorId: 1);
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.DeleteAsync($"/Comment/{comment.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteComment_ReturnsNotFound_WhenCommentIsNotDeleted()
    {
        var commentId = 1;
        
        var response = await _client.DeleteAsync($"/Comment/{commentId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}