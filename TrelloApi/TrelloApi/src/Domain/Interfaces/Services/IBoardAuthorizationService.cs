namespace TrelloApi.Domain.Interfaces.Services;

public interface IBoardAuthorizationService
{
    Task<bool> HasAccessToBoard(int uid, int boardId);
}