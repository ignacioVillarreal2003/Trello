using TrelloApi.Domain.Entities.Label;
using TrelloApi.Domain.Label.DTO;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ILabelService
{
    Task<OutputLabelDto?> GetLabelById(int labelId, int userId);
    Task<List<OutputLabelDto>> GetLabelsByTaskId(int taskId, int userId);
    Task<OutputLabelDto?> AddLabel(int boardId, AddLabelDto addLabelDto, int userId);
    Task<OutputLabelDto?> UpdateLabel(int labelId, UpdateLabelDto updateLabelDto, int userId);
    Task<OutputLabelDto?> DeleteLabel(int labelId, int userId);
}