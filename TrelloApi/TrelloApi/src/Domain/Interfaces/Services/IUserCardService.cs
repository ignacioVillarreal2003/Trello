using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IUserCardService
{
    Task<List<OutputUserDetailsDto>> GetUsersByCardId(int cardId, int uid);
    Task<OutputUserCardDetailsDto?> AddUserToCard(int cardId, AddUserCardDto dto, int uid);
    Task<Boolean> RemoveUserFromCard(int userId, int cardId, int uid);
}