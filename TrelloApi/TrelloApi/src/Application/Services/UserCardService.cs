using AutoMapper;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.Entities;
using TrelloApi.Domain.Interfaces.Repositories;
using TrelloApi.Domain.Interfaces.Services;

namespace TrelloApi.Application.Services;

public class UserCardService: BaseService, IUserCardService
{
    private readonly ILogger<UserCardService> _logger;
    private readonly IUserCardRepository _userCardRepository;
    
    public UserCardService(IMapper mapper, IBoardAuthorizationService boardAuthorizationService, ILogger<UserCardService> logger, IUserCardRepository userCardRepository) : base(mapper, boardAuthorizationService)
    {
        _logger = logger;
        _userCardRepository = userCardRepository;
    }

    public async Task<List<OutputUserDetailsDto>> GetUsersByCardId(int cardId, int uid)
    {
        try
        {
            List<User> users = await _userCardRepository.GetUsersByCardId(cardId);
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return _mapper.Map<List<OutputUserDetailsDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<OutputUserCardDetailsDto?> AddUserToCard(int cardId, AddUserCardDto dto, int uid)
    {
        try
        {
            UserCard userCard = new UserCard(dto.UserId, cardId);
            await _userCardRepository.AddUserCard(userCard);
            
            _logger.LogInformation("Card {CardId} added to User {UserId}", cardId, dto.UserId);
            return _mapper.Map<OutputUserCardDetailsDto>(userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to user {UserId}", cardId, dto.UserId);
            throw;
        }
    }

    public async Task<Boolean> RemoveUserFromCard(int userId, int cardId, int uid)
    {
        try
        {
            UserCard? userCard = await _userCardRepository.GetUserCardById(userId, cardId);
            if (userCard == null)
            {
                _logger.LogWarning("Card {CardId} to user {UserId} not found for deletion", cardId, userId);
                return false;
            }

            await _userCardRepository.DeleteUserCard(userCard);

            _logger.LogInformation("Card {CardId} to user {UserId} deleted", cardId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {CardId} to user {UserId}", cardId, userId);
            throw;
        }
    }
}