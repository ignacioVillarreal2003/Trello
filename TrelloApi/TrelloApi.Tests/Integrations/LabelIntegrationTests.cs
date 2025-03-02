using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class LabelIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public LabelIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetLabelById_ShouldReturnLabel_WhenLabelFound()
    {
        var label = new Label("title", "color", 1);
        
        _dbContext.Labels.Add(label);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/Label/{label.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLabelById_ShouldReturnNotFound_WhenLabelNotFound()
    {
        const int labelId = 1;
        
        var response = await _client.GetAsync($"/Label/{labelId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnLabels_WhenLabelsFound()
    {
        const int boardId = 1;
        var label1 = new Label("title 1", "color", boardId);
        var label2 = new Label("title 2", "color", boardId);
        
        _dbContext.Labels.AddRange(label1, label2);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/Label/board/{boardId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var labels = JsonSerializer.Deserialize<List<LabelResponse>>(body);

        Assert.NotNull(labels);
        Assert.Equal(2, labels.Count);
    }

    [Fact]
    public async Task GetLabelsByBoardId_ShouldReturnEmptyList_WhenLabelsNotFound()
    {
        const int boardId = 1;
        
        var response = await _client.GetAsync($"/Label/board/{boardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var labels = JsonSerializer.Deserialize<List<LabelResponse>>(body);

        Assert.NotNull(labels);
        Assert.Empty(labels);
    }

    [Fact]
    public async Task AddLabel_ShouldReturnCreated_WhenAddedSuccessful()
    {
        const int boardId = 1;
        var dto = new AddLabelDto { Title = "title", Color = "ff6575" };
        
        var response = await _client.PostAsJsonAsync($"/Label/board/{boardId}", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLabel_ShouldReturnOk_WhenUpdatedSuccessful()
    {
        var label = new Label("title", "color", 1);
        var dto = new UpdateLabelDto { Title = "updated title" };

        _dbContext.Labels.Add(label);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PutAsJsonAsync($"/Label/{label.Id}", dto);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLabel_ShouldReturnNotFound_WhenUpdatedUnsuccessful()
    {
        const int labelId = 1;
        var dto = new UpdateLabelDto { Title = "title" };
        
        var response = await _client.PutAsJsonAsync($"/Label/{labelId}", dto);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLabel_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var label = new Label("title", "color", 1);
        
        _dbContext.Labels.Add(label);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/Label/{label.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLabel_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int labelId = 1;
        
        var response = await _client.DeleteAsync($"/Label/{labelId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
