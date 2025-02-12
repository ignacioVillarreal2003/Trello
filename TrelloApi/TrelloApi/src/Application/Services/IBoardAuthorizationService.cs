namespace TrelloApi.Application.Services.Interfaces;

public interface IBoardAuthorizationService
{
    Task<bool> HasAccessToBoard(int userId, int boardId);
}