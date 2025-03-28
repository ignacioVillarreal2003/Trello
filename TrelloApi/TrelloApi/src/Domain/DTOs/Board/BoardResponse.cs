using TrelloApi.Domain.DTOs.Label;
using TrelloApi.Domain.DTOs.List;
using TrelloApi.Domain.DTOs.UserBoard;

namespace TrelloApi.Domain.DTOs.Board;

public class BoardResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<ListResponse> Lists;

    public ICollection<UserBoardResponse> UserBoards;

    public ICollection<LabelResponse> Labels;
}