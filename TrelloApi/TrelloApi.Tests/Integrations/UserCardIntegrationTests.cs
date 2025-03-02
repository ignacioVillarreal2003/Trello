using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class UserCardIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public UserCardIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnUsers_WhenUsersFound()
    {
        var card = new Card("title", "description", 1) { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var userCard = new UserCard(user.Id, card.Id);
        
        _dbContext.Cards.Add(card);
        _dbContext.Users.Add(user);
        _dbContext.UserCards.Add(userCard);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/UserCard/{card.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);

        Assert.NotNull(users);
        Assert.Single(users);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnEmptyList_WhenUsersNotFound()
    {
        const int cardId = 1;
        
        var response = await _client.GetAsync($"/UserCard/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task AddUserToCard_ShouldReturnCreated_WhenAddedSuccessful()
    {
        var card = new Card("title", "description", 1) { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var dto = new AddUserCardDto { UserId = user.Id };

        _dbContext.Cards.Add(card);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync($"/UserCard/card/{card.Id}", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromCard_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var card = new Card("title", "description", 1) { Id = 1 };
        var user = new User("user@email.com", "username", "password") { Id = 1 };
        var userCard = new UserCard(user.Id, card.Id);

        _dbContext.Cards.Add(card);
        _dbContext.Users.Add(user);
        _dbContext.UserCards.Add(userCard);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/UserCard/user/{user.Id}/card/{card.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromCard_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int userId = 1, cardId = 1;
        
        var response = await _client.DeleteAsync($"/UserCard/user/{userId}/card/{cardId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
