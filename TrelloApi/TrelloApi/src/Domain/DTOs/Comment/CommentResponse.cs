namespace TrelloApi.Domain.DTOs.Comment;

public class CommentResponse
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }        
    public int CardId { get; set; }
    public int AuthorId { get; set; }
}