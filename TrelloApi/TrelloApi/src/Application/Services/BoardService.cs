using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.Constants;
using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class BoardService: BaseService, IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserBoardRepository _userBoardRepository;
    private readonly ILogger<BoardService> _logger;
    
    public BoardService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IBoardRepository boardRepository, 
        IUserBoardRepository userBoardRepository, 
        ILogger<BoardService> logger) 
        : base(mapper, unitOfWork)
    {
        _boardRepository = boardRepository;
        _userBoardRepository = userBoardRepository;
        _logger = logger;
    }
    
    public async Task<BoardResponse?> GetBoardById(int boardId)
    {
        Board? board = await _boardRepository.GetAsync(b => b.Id.Equals(boardId));
        if (board == null)
        {
            _logger.LogWarning("Board {BoardId} not found", boardId);
            return null;
        }

        _logger.LogDebug("Board {BoardId} retrieved", boardId);
        return _mapper.Map<BoardResponse>(board);
    }
    
    public async Task<List<BoardResponse>> GetBoardsByUserId(int userId)
    {
        List<Board> boards = (await _boardRepository.GetBoardsByUserIdAsync(userId)).ToList();
        _logger.LogDebug("Retrieved {Count} boards for user {UserId}", boards.Count, userId);
        return _mapper.Map<List<BoardResponse>>(boards);
    }

    public async Task<BoardResponse> AddBoard(AddBoardDto dto, int userId)
    {
        Board board = new Board(dto.Title, dto.Background);
            
        await _boardRepository.CreateAsync(board);
        await _unitOfWork.CommitAsync();

        var userBoard = new UserBoard(userId, board.Id);
        await _userBoardRepository.CreateAsync(userBoard);

        await _unitOfWork.CommitAsync();
        _logger.LogInformation("Board added to user {UserId}", userId);
        return _mapper.Map<BoardResponse>(board);
    }
    
    public async Task<BoardResponse?> UpdateBoard(int boardId, UpdateBoardDto dto)
    {
        Board? board = await _boardRepository.GetAsync(b => b.Id.Equals(boardId));
        if (board == null)
        {
            _logger.LogWarning("Board {BoardId} not found for update", boardId);
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Title))
        {
            board.Title = dto.Title;
        }
        if (!string.IsNullOrEmpty(dto.Background))
        {
            board.Background = dto.Background;
        }
            
        await _boardRepository.UpdateAsync(board);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Board {BoardId} updated", boardId);
        return _mapper.Map<BoardResponse>(board);
    }
    
    public async Task<Boolean> DeleteBoard(int boardId)
    {
        Board? board = await _boardRepository.GetAsync(b => b.Id.Equals(boardId));
        if (board == null)
        {
            _logger.LogWarning("Board {BoardId} not found for deletion", boardId);
            return false;
        }

        await _boardRepository.DeleteAsync(board);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Board {BoardId} deleted", boardId);
        return true;
    }
    
    public async Task<BoardResponse> GetBoardByIdToAccess(int boardId)
    {
        Board board = await _boardRepository.GetBoardByIdToAccessAsync(boardId);
        _logger.LogInformation("Board {BoardId} access success", boardId);
        return _mapper.Map<BoardResponse>(board);
    }

    public async Task<BoardResponse?> GetBoardByIdComplete(int boardId)
    {
        Board? board = await _boardRepository.GetBoardByIdCompleteAsync(boardId);
        if (board == null)
        {
            _logger.LogWarning("Board {BoardId} not found", boardId);
            return null;
        }

        _logger.LogDebug("Board {BoardId} retrieved", boardId);
        return _mapper.Map<BoardResponse>(board);
    }
}