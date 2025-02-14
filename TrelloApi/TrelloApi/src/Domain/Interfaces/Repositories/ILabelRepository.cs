using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ILabelRepository
{
    Task<Label?> GetLabelById(int labelId);
    Task<List<Label>> GetLabelsByBoardId(int boardId);
    Task<Label?> AddLabel(Label label);
    Task<Label?> UpdateLabel(Label label);
    Task<bool> DeleteLabel(Label label);
}