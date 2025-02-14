using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ICardLabelService
{
    Task<List<OutputLabelDetailsDto>> GetLabelsByCardId(int cardId, int uid);
    Task<OutputCardLabelListDto?> AddLabelToCard(int cardId, int labelId, int uid);
    Task<bool> RemoveLabelFromCard(int cardId, int labelId, int uid);
}