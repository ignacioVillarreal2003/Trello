using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrelloApi.Domain.Entities;

[Table("User")]
public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(64), EmailAddress, Required]
    public string Email { get; set; }
    
    [StringLength(64), Required]
    public string Username { get; set; }
    
    [StringLength(256), Required, PasswordPropertyText]
    public string Password { get; set; }
    
    [StringLength(32), Required]
    public string Theme { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DataType(DataType.DateTime)] 
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
    
    public ICollection<UserBoard> UserBoards { get; set; } = new HashSet<UserBoard>();

    public ICollection<UserCard> UserTasks { get; set; } = new HashSet<UserCard>();
    
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public User(string email, string username, string password, string theme = "Light")
    {
        Email = email;
        Username = username;
        Password = password;
        Theme = theme;
    }
}