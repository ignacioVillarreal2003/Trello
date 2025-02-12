namespace TrelloApi.Domain.Entities.Comment;

public class OutputCommentDto
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public DateTime Date { get; set; }
    
    public string AuthorUsername { get; set; }
}