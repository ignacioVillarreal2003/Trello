using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserCardService
{
    Task<OutputUserCardDto?> GetUserCardById(int taskId, int uid);
    Task<OutputUserCardDto?> AddUserCard(AddUserCardDto addUserCardDto, int uid);
    Task<OutputUserCardDto?> DeleteUserCard(int userId, int taskId, int uid);
}