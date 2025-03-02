using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Data;

namespace TrelloApi.Tests.Integrations;

public class ListIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TrelloContext _dbContext;

    public ListIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        _scope = factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<TrelloContext>();

        var jwtService = _scope.ServiceProvider.GetRequiredService<IJwtService>();
        var token = jwtService.GenerateAccessToken(1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetListById_ShouldReturnList_WhenListFound()
    {
        var list = new List(title: "Test List", boardId: 1, position: 1);
        
        _dbContext.Lists.Add(list);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/List/{list.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetListById_ShouldReturnNotFound_WhenListNotFound()
    {
        const int listId = 1;
        
        var response = await _client.GetAsync($"/List/{listId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnLists_WhenListsFound()
    {
        const int boardId = 1;
        var list1 = new List(title: "List 1", boardId: 1, position: 1);
        var list2 = new List(title: "List 2", boardId: 1, position: 2);
        
        _dbContext.Lists.Add(list1);
        _dbContext.Lists.Add(list2);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.GetAsync($"/List/board/{boardId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var lists = JsonSerializer.Deserialize<List<ListResponse>>(body);
        
        Assert.NotNull(lists);
        Assert.Equal(2, lists.Count);
    }

    [Fact]
    public async Task GetListsByBoardId_ShouldReturnEmptyList_WhenListsNotFound()
    {
        const int boardId = 1;
        
        var response = await _client.GetAsync($"/List/board/{boardId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var body = await response.Content.ReadAsStringAsync();
        var lists = JsonSerializer.Deserialize<List<ListResponse>>(body);
        
        Assert.NotNull(lists);
        Assert.Empty(lists);
    }

    [Fact]
    public async Task AddList_ShouldReturnCreated_WhenListAddedSuccessfully()
    {
        const int boardId = 1;
        var dto = new AddListDto { Title = "title", Position = 1 };
        
        var response = await _client.PostAsJsonAsync($"/List/board/{boardId}", dto);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateList_ShouldReturnOk_WhenListUpdatedSuccessfully()
    {
        var list = new List(title: "title", boardId: 1, position: 1);
        var dto = new UpdateListDto { Title = "updated title" };
        
        _dbContext.Lists.Add(list);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.PutAsJsonAsync($"/List/{list.Id}", dto);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateList_ShouldReturnNotFound_WhenListNotFound()
    {
        const int listId = 1;
        var dto = new UpdateListDto { Title = "updated title" };
        
        var response = await _client.PutAsJsonAsync($"/List/{listId}", dto);
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteList_ShouldReturnNoContent_WhenDeletedSuccessful()
    {
        var list = new List(title: "title", boardId: 1, position: 1);
        
        _dbContext.Lists.Add(list);
        await _dbContext.SaveChangesAsync();
        
        var response = await _client.DeleteAsync($"/List/{list.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteList_ShouldReturnNotFound_WhenDeletedUnsuccessful()
    {
        const int listId = 1;
        
        var response = await _client.DeleteAsync($"/List/{listId}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
