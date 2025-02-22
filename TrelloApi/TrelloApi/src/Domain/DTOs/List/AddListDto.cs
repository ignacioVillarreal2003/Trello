namespace TrelloApi.Domain.DTOs.List;

public class AddListDto
{
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
}