using System.ComponentModel.DataAnnotations;

namespace TrelloApi.Domain.DTOs;

public class OutputBoardDetailsDto
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Background { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsArchived { get; set; }
    
    public DateTime? ArchivedAt { get; set; }
}

public class AddBoardDto
{
    [Required, StringLength(32)]
    public string Title { get; set; } = string.Empty;

    [StringLength(256)]
    public string? Description { get; set; }

    [Required, StringLength(32)]
    public string Background { get; set; } = string.Empty;
}

public class UpdateBoardDto
{
    [StringLength(32)]
    public string? Title { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }
    
    [Required, StringLength(32)]
    public string? Background { get; set; }
    
    public bool? IsArchived { get; set; }
}

