using FocusAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Data;

public class FocusContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<UserBadges> UserBadges { get; set; }
    public DbSet<UserPets> UserPets { get; set; }
    public DbSet<UserSessionHistory> UserSessionHistory { get; set; }
    public DbSet<UserFriends> Friends { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFriends>()
            .HasKey(x => new { x.UserId, x.FriendId, x.Status });

        modelBuilder.Entity<UserFriends>()
            .HasOne(x => x.User)
            .WithMany(x => x.Inviters)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserFriends>()
            .HasOne(x => x.Friend)
            .WithMany(x => x.Invitees)
            .HasForeignKey(x => x.FriendId);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // No SQL server conneciton yet
        // Better alternative for SQL Server connection https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\TestDb;Initial Catalog=FocusFriendsDevTest;Integrated Security=True;");
    }
}
