using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputCardLabelListDto
{
    public int CardId { get; set; }
        
    public int LabelId { get; set; }
}
    
public class AddCardLabelDto
{
    [Required]
    public int CardId { get; set; }
        
    [Required]
    public int LabelId { get; set; }
}