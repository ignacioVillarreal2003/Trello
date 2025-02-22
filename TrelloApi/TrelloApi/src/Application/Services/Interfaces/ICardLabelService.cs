using TrelloApi.Domain.DTOs.CardLabel;
using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Application.Services.Interfaces;

public interface ICardLabelService
{
    Task<List<LabelResponse>> GetLabelsByCardId(int cardId);
    Task<CardLabelResponse?> AddLabelToCard(int cardId, AddCardLabelDto dto);
    Task<bool> RemoveLabelFromCard(int cardId, int labelId);
}