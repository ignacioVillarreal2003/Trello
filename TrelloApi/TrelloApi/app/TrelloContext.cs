using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Board;
using TrelloApi.Domain.Comment;
using TrelloApi.Domain.Entities.List;
using TrelloApi.Domain.Entities.TaskLabel;
using TrelloApi.Domain.Entities.User;
using TrelloApi.Domain.Entities.UserTask;
using TrelloApi.Domain.Label;
using TrelloApi.Domain.User;
using TrelloApi.Domain.UserBoard;
using TrelloApi.Domain.UserTask;
using Task = TrelloApi.Domain.Task.Task;

namespace TrelloApi.app;

public class TrelloContext: DbContext
{
    public TrelloContext(DbContextOptions<TrelloContext> options) : base(options)
    {
        
    }
    
    public DbSet<Board> Boards { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserBoard> UserBoards { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
    public DbSet<TaskLabel> TaskLabels { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>().ToTable("Board");
        modelBuilder.Entity<Comment>().ToTable("Comment");
        modelBuilder.Entity<Label>().ToTable("Label");
        modelBuilder.Entity<List>().ToTable("List");
        modelBuilder.Entity<Task>().ToTable("Task");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<UserBoard>().ToTable("UserBoard");
        modelBuilder.Entity<UserTask>().ToTable("UserTask");
        modelBuilder.Entity<TaskLabel>().ToTable("TaskLabel");

        
        modelBuilder.Entity<UserBoard>()
            .HasKey(ub => new { ub.UserId, ub.BoardId });
        modelBuilder.Entity<UserTask>()
            .HasKey(ut => new { ut.UserId, ut.TaskId });
        modelBuilder.Entity<TaskLabel>()
            .HasKey(tl => new { tl.TaskId, tl.LabelId });
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}