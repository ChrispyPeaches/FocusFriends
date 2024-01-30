using FocusCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Data;

public class FocusContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Badges> Badges { get; set; }
    public DbSet<Pets> Pets { get; set; }
    public DbSet<UserBadges> UserBadges { get; set; }
    public DbSet<UserPets> UserPets { get; set; }
    public DbSet<UserSessionHistory> UserSessionHistory { get; set; }
    public DbSet<UserFriends> UserFriends { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Bad practice
        // Better alternative in https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
        optionsBuilder.UseSqlServer(@"Sql Sever Connection String Goes here");
    }
}
