using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Board;
using TrelloApi.Domain.Entities.Board;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;
using TrelloApi.Domain.UserBoard.DTO;

namespace TrelloApi.Application.Services;

public class BoardService: BaseService, IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserBoardService _userBoardService;
    private readonly ILogger<BoardService> _logger;
    
    public BoardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, IUserBoardService userBoardService, IBoardRepository boardRepository, ILogger<BoardService> logger) : base(mapper, boardAuthorizationService)
    {
        _userBoardService = userBoardService;
        _boardRepository = boardRepository;
        _logger = logger;
    }
    
    public async Task<OutputBoardDto?> GetBoardById(int boardId, int userId)
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
            return _mapper.Map<OutputBoardDto>(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board {BoardId}", boardId);
            throw;
        }
    }
    
    public async Task<List<OutputBoardDto>> GetBoards(int userId)
    {
        try
        {
            List<Board> boards = await _boardRepository.GetBoards(userId);
            _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, userId);
            return _mapper.Map<List<OutputBoardDto>>(boards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving boards for user {UserId}", userId);
            throw;
        }
    }

    public async Task<OutputBoardDto?> AddBoard(AddBoardDto addBoardDto, int userId)
    {
        try
        {
            Board board = new Board(addBoardDto.Title, addBoardDto.Theme, addBoardDto.Icon);
            Board? newBoard = await _boardRepository.AddBoard(board);
            if (newBoard == null)
            {
                _logger.LogError("Failed to add board to user {UserId}", userId);
                return null;
            }
            
            AddUserBoardDto userBoard = new AddUserBoardDto
            {
                BoardId = newBoard.Id,
                UserId = userId
            };
            await _userBoardService.AddUserBoard(userBoard, userId);

            _logger.LogInformation("Board added to user {UserId}", userId);
            return _mapper.Map<OutputBoardDto>(newBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding board to user {UserId}", userId);
            throw;
        }
    }
    
    public async Task<OutputBoardDto?> UpdateBoard(int boardId, UpdateBoardDto updateBoardDto, int userId)
    {
        try
        {
            Board? board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogWarning("Board {BoardId} not found for update", boardId);
                return null;
            }

            if (!string.IsNullOrEmpty(updateBoardDto.Title))
            {
                board.Title = updateBoardDto.Title;
            }
            if (!string.IsNullOrEmpty(updateBoardDto.Theme))
            {
                board.Theme = updateBoardDto.Theme;
            }
            if (!string.IsNullOrEmpty(updateBoardDto.Icon))
            {
                board.Icon = updateBoardDto.Icon;
            }

            Board? updatedBoard = await _boardRepository.UpdateBoard(board);
            if (updatedBoard == null)
            {
                _logger.LogError("Failed to update board {BoardId}", boardId);
                return null;
            }
            
            _logger.LogInformation("Board {BoardId} updated", boardId);
            return _mapper.Map<OutputBoardDto>(updatedBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating board {BoardId}", boardId);
            throw;
        }
    }

    public async Task<OutputBoardDto?> DeleteBoard(int boardId, int userId)
    {
        try
        {
            Board? board = await _boardRepository.GetBoardById(boardId);
            if (board == null)
            {
                _logger.LogWarning("Board {BoardId} not found for deletion", boardId);
                return null;
            }

            Board? deletedBoard = await _boardRepository.DeleteBoard(board);
            if (deletedBoard == null)
            {
                _logger.LogError("Failed to delete board {BoardId}", boardId);
                return null;
            }
            
            _logger.LogInformation("Board {BoardId} deleted", boardId);
            return _mapper.Map<OutputBoardDto>(deletedBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board {BoardId}", boardId);
            throw;
        }
    }
}