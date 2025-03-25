namespace TrelloApi.Application.Hub;

public class BoardHub: Microsoft.AspNetCore.SignalR.Hub
{
    public async Task JoinBoardGroup(string boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
    }
    
    public async Task LeaveBoardGroup(string boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
    }
}