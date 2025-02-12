namespace TrelloApi.Domain.Interfaces.Repositories;

public interface ILabelRepository
{
    Task<Label.Label?> GetLabelById(int labelId);
    Task<List<Label.Label>> GetLabelsByTaskId(int taskId);
    Task<Label.Label?> AddLabel(Label.Label label);
    Task<Label.Label?> UpdateLabel(Label.Label label);
    Task<Label.Label?> DeleteLabel(Label.Label label);
}