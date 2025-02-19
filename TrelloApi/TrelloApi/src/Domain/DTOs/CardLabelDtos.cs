using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputCardLabelDetailsDto
{
    public int CardId { get; set; }
        
    public int LabelId { get; set; }
}

public class AddCardLabelDto
{
    [Required]
    public int LabelId { get; set; }
}