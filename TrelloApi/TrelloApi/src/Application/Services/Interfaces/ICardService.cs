using TrelloApi.Domain.DTOs.Card;

namespace TrelloApi.Application.Services.Interfaces;

public interface ICardService
{
    Task<CardResponse?> GetCardById(int cardId);
    Task<List<CardResponse>> GetCardsByListId(int listId);
    Task<CardResponse?> AddCard(int listId, AddCardDto dto);
    Task<CardResponse?> UpdateCard(int cardId, UpdateCardDto dto);
    Task<Boolean> DeleteCard(int cardId);
}