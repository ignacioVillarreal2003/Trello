using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ICardLabelService
{
    Task<List<OutputLabelDetailsDto>> GetLabelsByCardId(int cardId, int uid);
    Task<OutputCardLabelDetailsDto?> AddLabelToCard(int cardId, AddCardLabelDto dto, int uid);
    Task<bool> RemoveLabelFromCard(int cardId, int labelId, int uid);
}