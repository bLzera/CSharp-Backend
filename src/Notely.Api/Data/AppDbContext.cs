using Microsoft.EntityFrameworkCore;
using Notely.Api.Models;

namespace Notely.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        builder.Entity<Note>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Title).IsRequired().HasMaxLength(255);
            e.Property(n => n.Content).IsRequired();
            e.HasOne(n => n.User)
             .WithMany(u => u.Notes)
             .HasForeignKey(n => n.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
