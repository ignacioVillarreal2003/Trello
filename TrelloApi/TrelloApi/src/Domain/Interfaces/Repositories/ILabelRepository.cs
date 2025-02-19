using TrelloApi.Domain.Entities;

namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ILabelRepository
{
    Task<Label?> GetLabelById(int labelId);
    Task<List<Label>> GetLabelsByBoardId(int boardId);
    Task AddLabel(Label label);
    Task UpdateLabel(Label label);
    Task DeleteLabel(Label label);
}