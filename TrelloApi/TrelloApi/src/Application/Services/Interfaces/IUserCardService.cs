using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;

namespace TrelloApi.Application.Services.Interfaces;

public interface IUserCardService
{
    Task<List<UserResponse>> GetUsersByCardId(int cardId);
    Task<UserCardResponse> AddUserToCard(int cardId, AddUserCardDto dto);
    Task<Boolean> RemoveUserFromCard(int userId, int cardId);
}