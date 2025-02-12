using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.Comment;

public class AddCommentDto
{
    [StringLength(256), Required] 
    public string Text { get; set; }
    
    public required int AuthorId { get; set; }
}