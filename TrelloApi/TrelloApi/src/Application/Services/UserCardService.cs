using AutoMapper;
using TrelloApi.Application.Services.Interfaces;
using TrelloApi.Domain.DTOs;
using TrelloApi.Domain.DTOs.User;
using TrelloApi.Domain.DTOs.UserCard;
using TrelloApi.Domain.Entities;
using TrelloApi.Infrastructure.Persistence.Interfaces;

namespace TrelloApi.Application.Services;

public class UserCardService: BaseService, IUserCardService
{
    private readonly ILogger<UserCardService> _logger;
    private readonly IUserCardRepository _userCardRepository;
    
    public UserCardService(IMapper mapper, 
        IUnitOfWork unitOfWork,
        ILogger<UserCardService> logger, 
        IUserCardRepository userCardRepository) 
        : base(mapper, unitOfWork)
    {
        _logger = logger;
        _userCardRepository = userCardRepository;
    }

    public async Task<List<UserResponse>> GetUsersByCardId(int cardId)
    {
        try
        {
            List<User> users = (await _userCardRepository.GetUsersByCardIdAsync(cardId)).ToList();
            _logger.LogDebug("Retrieved {Count} users for card {CardId}", users.Count, cardId);
            return _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for card {CardId}", cardId);
            throw;
        }
    }

    public async Task<UserCardResponse> AddUserToCard(int cardId, AddUserCardDto dto)
    {
        try
        {
            UserCard userCard = new UserCard(dto.UserId, cardId);
            await _userCardRepository.CreateAsync(userCard);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Card {CardId} added to User {UserId}", cardId, dto.UserId);
            return _mapper.Map<UserCardResponse>(userCard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding card {CardId} to user {UserId}", cardId, dto.UserId);
            throw;
        }
    }

    public async Task<Boolean> RemoveUserFromCard(int userId, int cardId)
    {
        try
        {
            UserCard? userCard = await _userCardRepository.GetAsync(uc => uc.UserId.Equals(userId) && uc.CardId.Equals(cardId));
            if (userCard == null)
            {
                _logger.LogWarning("Card {CardId} to user {UserId} not found for deletion", cardId, userId);
                return false;
            }

            await _userCardRepository.DeleteAsync(userCard);
            await _unitOfWork.CommitAsync();

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