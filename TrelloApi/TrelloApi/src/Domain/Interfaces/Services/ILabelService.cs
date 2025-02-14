using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface ILabelService
{
    Task<OutputLabelDetailsDto?> GetLabelById(int labelId, int uid);
    Task<List<OutputLabelDetailsDto>> GetLabelsByBoardId(int boardId, int uid);
    Task<OutputLabelDetailsDto?> AddLabel(int boardId, AddLabelDto dto, int uid);
    Task<OutputLabelDetailsDto?> UpdateLabel(int labelId, UpdateLabelDto dto, int uid);
    Task<bool> DeleteLabel(int labelId, int uid);
}