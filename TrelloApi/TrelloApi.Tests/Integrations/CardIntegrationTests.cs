using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.Card;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class CardIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public CardIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetCardById_ShouldReturnCard_WhenCardFound()
    {
        var card = new Card("title", "description", 1);
        
        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/Card/{card.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCardById_ShouldReturnNotFound_WhenCardNotFound()
    {
        const int cardId = 1;
        
        var response = await _client.GetAsync($"/Card/{cardId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnCards_WhenCardsFound()
    {
        const int listId = 1;
        var card1 = new Card("title 1", "description", listId);
        var card2 = new Card("title 2", "description", listId);
        
        _dbContext.Cards.AddRange(card1, card2);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/Card/list/{listId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var cards = JsonSerializer.Deserialize<List<CardResponse>>(body);

        Assert.NotNull(cards);
        Assert.Equal(2, cards.Count);
    }

    [Fact]
    public async Task GetCardsByListId_ShouldReturnEmptyList_WhenCardsNotFound()
    {
        const int listId = 1;

        var response = await _client.GetAsync($"/Card/list/{listId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var cards = JsonSerializer.Deserialize<List<CardResponse>>(body);

        Assert.NotNull(cards);
        Assert.Empty(cards);
    }

    [Fact]
    public async Task AddCard_ShouldReturnCreated_WhenAddedSuccessful()
    {
        const int listId = 1;
        var dto = new AddCardDto { Title = "title", Description = "description", Priority = "Medium" };
        
        var response = await _client.PostAsJsonAsync($"/Card/list/{listId}", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCard_ShouldReturnOk_WhenUpdatedSuccessful()
    {
        var card = new Card("title", "description", 1);
        var dto = new UpdateCardDto { Title = "updated title" };

        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PutAsJsonAsync($"/Card/{card.Id}", dto);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCard_ShouldReturnBadRequest_WhenUpdatedUnsuccessful()
    {
        const int cardId = 1;
        var dto = new UpdateCardDto { Title = "updated title" };
        
        var response = await _client.PutAsJsonAsync($"/Card/{cardId}", dto);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCard_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var card = new Card("title", "description", 1);
        
        _dbContext.Cards.Add(card);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/Card/{card.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCard_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int cardId = 1;
        
        var response = await _client.DeleteAsync($"/Card/{cardId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}