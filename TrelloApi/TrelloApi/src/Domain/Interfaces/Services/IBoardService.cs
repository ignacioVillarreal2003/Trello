using TrelloApi.Domain.DTOs;

namespace TrelloApi.Domain.Interfaces.Services;

public interface IBoardService
{
    Task<OutputBoardDetailsDto?> GetBoardById(int boardId, int uid);
    Task<List<OutputBoardDetailsDto>> GetBoardsByUserId(int uid);
    Task<OutputBoardDetailsDto?> AddBoard(AddBoardDto dto, int uid);
    Task<OutputBoardDetailsDto?> UpdateBoard(int boardId, UpdateBoardDto dto, int uid);
    Task<bool> DeleteBoard(int boardId, int uid);
}