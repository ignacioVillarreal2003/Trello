using Microsoft.EntityFrameworkCore;
using TrelloApi.Domain.Entities;

namespace TrelloApi.Infrastructure.Persistence.Data;

public class TrelloContext: DbContext
{
    public TrelloContext(DbContextOptions<TrelloContext> options) : base(options)
    {
        
    }
    
    public DbSet<Board> Boards { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserBoard> UserBoards { get; set; }
    public DbSet<UserCard> UserCards { get; set; }
    public DbSet<CardLabel> CardLabels { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>().ToTable("Board");
        modelBuilder.Entity<Comment>().ToTable("Comment");
        modelBuilder.Entity<Label>().ToTable("Label");
        modelBuilder.Entity<List>().ToTable("List");
        modelBuilder.Entity<Card>().ToTable("Card");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<UserBoard>().ToTable("UserBoard");
        modelBuilder.Entity<UserCard>().ToTable("UserCard");
        modelBuilder.Entity<CardLabel>().ToTable("CardLabel");

        
        modelBuilder.Entity<UserBoard>()
            .HasKey(ub => new { ub.UserId, ub.BoardId });
        modelBuilder.Entity<UserCard>()
            .HasKey(uc => new { uc.UserId, uc.CardId });
        modelBuilder.Entity<CardLabel>()
            .HasKey(cl => new { cl.CardId, cl.LabelId });
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        foreach (var item in ChangeTracker.Entries<Entity>().AsEnumerable())
        {
            if (item.State == EntityState.Added)
            {
                item.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (item.State == EntityState.Modified)
            {
                item.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}