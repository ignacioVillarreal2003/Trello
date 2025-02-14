using AutoMapper;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class BoardService: BaseService, IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<BoardService> _logger;
    
    public BoardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, IBoardRepository boardRepository, IUserBoardRepository userBoardRepository, ILogger<BoardService> logger) : base(mapper, boardAuthorizationService)
    {
        _boardRepository = boardRepository;
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    }
    
    public async Task<OutputBoardDetailsDto?> GetBoardById(int boardId, int uid)
    {
        try
        {
            Board? board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogWarning("Board {BoardId} not found", boardId);
                return null;
            }

            _logger.LogDebug("Board {BoardId} retrieved", boardId);
            return _mapper.Map<OutputBoardDetailsDto>(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board {BoardId}", boardId);
            throw;
        }
    }
    
    public async Task<List<OutputBoardListDto>> GetBoardsByUserId(int uid)
    {
        try
        {
            List<Board> boards = await _boardRepository.GetBoardsByUserId(uid);
            _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, uid);
            return _mapper.Map<List<OutputBoardListDto>>(boards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving boards for user {UserId}", uid);
            throw;
        }
    }

    public async Task<OutputBoardDetailsDto?> AddBoard(AddBoardDto dto, int uid)
    {
        try
        {
            if (!BoardColorValues.BoardColorsAllowed.Contains(dto.Color))
            {
                return null;
            }
            
            Board board = new Board(dto.Title, dto.Color);
            
            if (!string.IsNullOrEmpty(dto.Description))
            {
                board.Description = dto.Description;
            }
            
            Board? newBoard = await _boardRepository.AddBoard(board);
            if (newBoard == null)
            {
                _logger.LogError("Failed to add board to user {UserId}", uid);
                return null;
            }

            var userBoard = new UserBoard(uid, newBoard.Id);
            await _userBoardRepository.AddUserBoard(userBoard);
            
            _logger.LogInformation("Board added to user {UserId}", uid);
            return _mapper.Map<OutputBoardDetailsDto>(newBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding board to user {UserId}", uid);
            throw;
        }
    }
    
    public async Task<OutputBoardDetailsDto?> UpdateBoard(int boardId, UpdateBoardDto dto, int uid)
    {
        try
        {
            Board? board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogWarning("Board {BoardId} not found for update", boardId);
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Title))
            {
                board.Title = dto.Title;
            }
            if (!string.IsNullOrEmpty(dto.Color))
            {
                board.Color = dto.Color;
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                board.Description = dto.Description;
            }
            if (dto.IsArchived != null)
            {
                board.IsArchived = dto.IsArchived.Value;
                board.ArchivedAt = DateTime.UtcNow;
            }
            board.UpdatedAt = DateTime.UtcNow;

            Board? updatedBoard = await _boardRepository.UpdateBoard(board);
            if (updatedBoard == null)
            {
                _logger.LogError("Failed to update board {BoardId}", boardId);
                return null;
            }
            
            _logger.LogInformation("Board {BoardId} updated", boardId);
            return _mapper.Map<OutputBoardDetailsDto>(updatedBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating board {BoardId}", boardId);
            throw;
        }
    }
    
    public async Task<bool> DeleteBoard(int boardId, int uid)
    {
        try
        {
            Board? board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogWarning("Board {BoardId} not found for deletion", boardId);
                return false;
            }

            Board? deletedBoard = await _boardRepository.DeleteBoard(board);
            if (deletedBoard == null)
            {
                _logger.LogError("Failed to delete board {BoardId}", boardId);
                return false;
            }
            
            _logger.LogInformation("Board {BoardId} deleted", boardId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board {BoardId}", boardId);
            throw;
        }
    }
}