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

    public async Task<OutputUserCardDto?> GetUserCardById(int cardId, int uid)
    {
        try
        {
            UserCard? userCard = await _userCardRepository.GetUserCardById(cardId, uid);
            if (userCard == null)
            {
                _logger.LogWarning("Card {CardId} for user {UserId} not found.", cardId, uid);
                return null;
            }

            _logger.LogDebug("Task {CardId} for card {UserId} retrieved", cardId, uid);
            return _mapper.Map<OutputUserCardDto>(userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving card {CardId} for user {UserId}", cardId, uid);
            throw;
        }
    }

    public async Task<OutputUserCardDto?> AddUserCard(AddUserCardDto dto, int uid)
    {
        try
        {
            UserCard userCard = new UserCard(dto.UserId, dto.CardId);
            UserCard? newUserCard = await _userCardRepository.AddUserCard(userCard);
            if (newUserCard == null)
            {
                _logger.LogError("Failed to add card {CardId} to user {UserId}", dto.CardId, dto.UserId);
                return null;
            }
            
            _logger.LogInformation("Card {CardId} added to User {UserId}", dto.CardId, dto.UserId);
            return _mapper.Map<OutputUserCardDto>(newUserCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to user {UserId}", dto.CardId, dto.UserId);
            throw;
        }
    }

    public async Task<OutputUserCardDto?> DeleteUserCard(int userId, int cardId, int uid)
    {
        try
        {
            UserCard? userCard = await _userCardRepository.GetUserCardById(userId, cardId);
            if (userCard == null)
            {
                _logger.LogWarning("Card {CardId} to user {UserId} not found for deletion", cardId, userId);
                return null;
            }

            UserCard? deletedUserCard = await _userCardRepository.DeleteUserCard(userCard);
            if (deletedUserCard == null)
            {
                _logger.LogError("Failed to delete card {CardId} to user {UserId}", cardId, userId);
                return null;
            }

            _logger.LogInformation("Card {CardId} to user {UserId} deleted", cardId, userId);
            return _mapper.Map<OutputUserCardDto>(deletedUserCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {CardId} to user {UserId}", cardId, userId);
            throw;
        }
    }
}