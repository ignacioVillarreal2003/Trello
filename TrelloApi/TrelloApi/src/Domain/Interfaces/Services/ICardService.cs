using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ICardService
{
    Task<OutputCardDetailsDto?> GetCardById(int cardId, int uid);
    Task<List<OutputCardListDto>> GetCardsByListId(int listId, int uid);
    Task<OutputCardDetailsDto?> AddCard(int listId, AddCardDto addTaskDto, int uid);
    Task<OutputCardDetailsDto?> UpdateCard(int cardId, UpdateCardDto updateTaskDto, int uid);
    Task<Boolean> DeleteCard(int cardId, int uid);
}