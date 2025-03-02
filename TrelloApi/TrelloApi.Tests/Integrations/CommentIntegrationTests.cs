using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Comment;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

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
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetCommentById_ShouldReturnsComment_WhenCommentFound()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1);
        
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Comment/{comment.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCommentById_ShouldReturnsNotFound_WhenCommentNotFound()
    {
        const int commentId = 1;
        
        var response = await _client.GetAsync($"/Comment/{commentId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnsComments_WhenCommentsFound()
    {
        const int cardId = 1;
        var comment1 = new Comment(text: "text 1", cardId: 1, authorId: 1);
        var comment2 = new Comment(text: "text 2", cardId: 1, authorId: 1);
        
        _dbContext.Comments.Add(comment1);
        _dbContext.Comments.Add(comment2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/Comment/card/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var comments = JsonSerializer.Deserialize<List<CommentResponse>>(body);

        Assert.NotNull(comments);
        Assert.Equal(2, comments.Count);
    }
    
    [Fact]
    public async Task GetCommentsByCardId_ShouldReturnsEmptyList_WhenCommentsNotFound()
    {
        const int cardId = 1;
        
        var response = await _client.GetAsync($"/Comment/card/{cardId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var comments = JsonSerializer.Deserialize<List<CommentResponse>>(body);
        
        Assert.NotNull(comments);
        Assert.Empty(comments);
    }
    
    [Fact]
    public async Task AddComment_ShouldReturnsCreated_WhenAddedSuccessful()
    {
        const int cardId = 1;
        var dto = new { Text = "text", AuthorId = 1 };
        
        var response = await _client.PostAsJsonAsync($"/Comment/card/{cardId}", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateComment_ShouldReturnsOk_WhenUpdatedSuccessful()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1);
        
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var dto = new { text = "updated text" };
        
        var response = await _client.PutAsJsonAsync($"/Comment/{comment.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateComment_ShouldReturnsBadRequest_WhenUpdatedUnsuccessful()
    {
        const int commentId = 1;
        var dto = new { title = "updated title" };
        
        var response = await _client.PutAsJsonAsync($"/Comment/{commentId}", dto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteComment_ShouldReturnsNoContent_WhenDeletedSuccessful()
    {
        var comment = new Comment(text: "text", cardId: 1, authorId: 1);
        
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.DeleteAsync($"/Comment/{comment.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteComment_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
    {
        const int commentId = 1;
        
        var response = await _client.DeleteAsync($"/Comment/{commentId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}