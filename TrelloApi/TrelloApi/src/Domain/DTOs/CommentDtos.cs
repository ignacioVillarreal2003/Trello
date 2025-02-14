using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputCommentDetailsDto
{
    public int Id { get; set; }
        
    public string Text { get; set; } = string.Empty;
        
    public DateTime Date { get; set; }
        
    public int CardId { get; set; }
        
    public int AuthorId { get; set; }
}

public class AddCommentDto
{
    [Required, StringLength(256)]
    public string Text { get; set; } = string.Empty;
        
    [Required]
    public int CardId { get; set; }
        
    [Required]
    public int AuthorId { get; set; }
}

public class UpdateCommentDto
{
    [Required, StringLength(256)]
    public string Text { get; set; } = string.Empty;
}
