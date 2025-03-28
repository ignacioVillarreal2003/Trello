using TrelloApi.Domain.DTOs.Board;
using TrelloApi.Domain.DTOs.Card;

namespace TrelloApi.Domain.DTOs.List;

public class ListResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public int BoardId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public BoardResponse Board;
    public ICollection<CardResponse> Cards;
}