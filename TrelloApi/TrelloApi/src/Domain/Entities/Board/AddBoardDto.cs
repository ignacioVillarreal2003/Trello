using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.Board;

public class AddBoardDto
{
    [StringLength(64), Required]
    public string Title { get; set; }
    
    [StringLength(32), Required]
    public string Theme { get; set; }
    
    [StringLength(64), Required]
    public string Icon { get; set; }
    
    [StringLength(256)]
    public string? Description { get; set; }
}