using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.Entities.Board;

public class UpdateBoardDto
{
    [StringLength(64)]
    public string? Title { get; set; }
    
    [StringLength(32)]
    public string? Theme { get; set; }
    
    [StringLength(64)]
    public string? Icon { get; set; }
    
    [StringLength(256)]
    public string? Description { get; set; }
}