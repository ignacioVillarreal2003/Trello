using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class CardLabelIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public CardLabelIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnLabels_WhenLabelsFound()
    {
        var card = new Card("title", "description",1) { Id = 1 };
        var label = new Label("title", "color", 1) { Id = 1 };
        var cardLabel = new CardLabel (card.Id, label.Id);

        _dbContext.Cards.Add(card);
        _dbContext.Labels.Add(label);
        _dbContext.CardLabels.Add(cardLabel);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/CardLabel/card/{card.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var labels = JsonSerializer.Deserialize<List<LabelResponse>>(body);

        Assert.NotNull(labels);
        Assert.Single(labels);
    }

    [Fact]
    public async Task GetLabelsByCardId_ShouldReturnEmptyList_WhenLabelsNotFound()
    {
        const int cardId = 1;
        
        var response = await _client.GetAsync($"/CardLabel/card/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var labels = JsonSerializer.Deserialize<List<LabelResponse>>(body);
        
        Assert.NotNull(labels);
        Assert.Empty(labels);
    }

    [Fact]
    public async Task AddLabelToCard_ShouldReturnCreated_WhenAddedSuccessful()
    {
        var card = new Card("title", "description",1) { Id = 1 };
        var label = new Label("title", "color", 1) { Id = 1 };
        var dto = new AddCardLabelDto { LabelId = label.Id };

        _dbContext.Cards.Add(card);
        _dbContext.Labels.Add(label);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync($"/CardLabel/card/{card.Id}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var i = 1;
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RemoveLabelFromCard_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var card = new Card("title", "description",1) { Id = 1 };
        var label = new Label("title", "color", 1) { Id = 1 };
        var cardLabel = new CardLabel (card.Id, label.Id);

        _dbContext.Cards.Add(card);
        _dbContext.Labels.Add(label);
        _dbContext.CardLabels.Add(cardLabel);
        
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/CardLabel/card/{card.Id}/label/{label.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveLabelFromCard_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int cardId = 1, labelId = 1;
        
        var response = await _client.DeleteAsync($"/CardLabel/card/{cardId}/label/{labelId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
