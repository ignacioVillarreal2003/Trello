using TrelloApi.Domain.DTOs.Label;

namespace TrelloApi.Application.Services.Interfaces;

public interface ILabelService
{
    Task<LabelResponse?> GetLabelById(int labelId);
    Task<List<LabelResponse>> GetLabelsByBoardId(int boardId);
    Task<LabelResponse?> AddLabel(int boardId, AddLabelDto dto);
    Task<LabelResponse?> UpdateLabel(int labelId, UpdateLabelDto dto);
    Task<bool> DeleteLabel(int labelId);
}