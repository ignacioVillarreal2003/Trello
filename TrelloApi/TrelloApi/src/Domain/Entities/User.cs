using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("User")]
public class User: Entity
{
    [StringLength(64), EmailAddress, Required]
    public string Email { get; set; }
    
    [StringLength(64), Required]
    public string Username { get; set; }
    
    [StringLength(256), Required, PasswordPropertyText]
    public string Password { get; set; }
    
    [StringLength(32), Required]
    public string Theme { get; set; }

    [DataType(DataType.DateTime)] 
    public DateTime LastLogin { get; set; }
    
    public string? RefreshToken { get; set; }
    
    [DataType(DataType.DateTime)] 
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public ICollection<UserBoard> UserBoards { get; set; }

    public ICollection<UserCard> UserCards { get; set; }
    
    public ICollection<Comment> Comments { get; set; }

    public User(string email, string username, string password, string theme = "Light")
    {
        Email = email;
        Username = username;
        Password = password;
        Theme = theme;
        CreatedAt = DateTime.UtcNow;
        LastLogin = DateTime.UtcNow;
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }
}