using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Application.Utils;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class UserIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;
    private readonly IEncrypt _encrypt;

    public UserIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        
        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        _encrypt = new Encrypt();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WhenUsersFound()
    {
        var user1 = new User("user1@email.com", "username", "password", "theme") { Id = 1 };
        var user2 = new User("user2@email.com", "username", "password", "theme") { Id = 2 };

        _dbContext.Users.Add(user1);
        _dbContext.Users.Add(user2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync("/User");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(users);
        Assert.Equal(2, users.Count);
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenUsersNotFound()
    {
        var response = await _client.GetAsync("/User");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
        var body = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(users);
        Assert.Empty(users);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ShouldReturnUsers_WhenUsersFound()
    {
        const string username = "user";
        var user1 = new User("user1@email.com", "username", "password", "theme") { Id = 1 };
        var user2 = new User("user2@email.com", "username", "password", "theme") { Id = 2 };

        _dbContext.Users.Add(user1);
        _dbContext.Users.Add(user2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/User/username/{username}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Equal(2, boards.Count);
    }
    
    [Fact]
    public async Task GetUsersByUsername_ShouldReturnEmptyList_WhenUsersNotFound()
    {
        const string username = "user";
        
        var response = await _client.GetAsync($"/User/username/{username}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Empty(boards);
    }

    [Fact]
    public async Task GetUsersByCardId_ShouldReturnUsers_WhenUsersFound()
    {
        const int cardId = 1;
        var user1 = new User("user1@email.com", "username", "password", "theme") { Id = 1 };
        var user2 = new User("user2@email.com", "username", "password", "theme") { Id = 2 };
        var card = new Card("title", "description", 1) {Id = cardId};
        var userCard1 = new UserCard(user1.Id, card.Id);
        var userCard2 = new UserCard(user2.Id, card.Id);

        _dbContext.Users.Add(user1);
        _dbContext.Users.Add(user2);
        _dbContext.Cards.Add(card);
        _dbContext.UserCards.Add(userCard1);
        _dbContext.UserCards.Add(userCard2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/User/card/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Equal(2, boards.Count);
    }
    
    [Fact]
    public async Task GetUsersByCardId_ShouldReturnEmptyList_WhenUsersNotFound()
    {
        const int cardId = 1;
        
        var response = await _client.GetAsync($"/User/card/{cardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var boards = JsonSerializer.Deserialize<List<UserResponse>>(body);
        
        Assert.NotNull(boards);
        Assert.Empty(boards);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnCreated_WhenRegisteredSuccessful()
    {
        var dto = new RegisterUserDto { Email = "user@email.com", Username = "username", Password = "password" };
        
        var response = await _client.PostAsJsonAsync("/User/register", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnsOk_WhenUserFound()
    {
        var password = _encrypt.HashPassword("password");   
        var user = new User("user@email.com", "username", password, "theme") { Id = 1 };
        var dto = new LoginUserDto { Email = user.Email, Password = "password" };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync("/User/login", dto);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnsBadRequest_WhenUserNotFound()
    {
        var dto = new LoginUserDto { Email = "user@email.com", Password = "password" };
        
        var response = await _client.PostAsJsonAsync("/User/login", dto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    
    [Fact]
    public async Task Login_ShouldReturnsUnauthorized_WhenUserCredentialsAreIncorrect()
    {
        var user = new User("user@email.com", "username", "password", "theme") { Id = 1 };
        var dto = new LoginUserDto { Email = "user@email.com", Password = "incorrect password" };
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.PostAsJsonAsync("/User/login", dto);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnsOk_WhenUpdatedSuccessful()
    {
        var user = new User("user@email.com", "username", "password", "theme") { Id = 1 };
        var dto = new UpdateUserDto { Username = "updated username" };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _client.PutAsJsonAsync("/User", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnsNotFound_WhenUpdatedUnsuccessful()
    {
        var dto = new UpdateUserDto { Username = "updated username" };
        
        var response = await _client.PutAsJsonAsync("/User", dto);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnsNoContent_WhenSuccessful()
    {
        var user = new User("user@email.com", "username", "password", "theme") { Id = 1 };
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync("/User");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnsNotFound_WhenDeletedUnsuccessful()
    {
        var response = await _client.DeleteAsync("/User");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
