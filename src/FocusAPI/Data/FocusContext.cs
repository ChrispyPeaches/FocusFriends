using FocusAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace FocusAPI.Data;

public class FocusContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<UserBadge> UserBadges { get; set; }
    public DbSet<UserPet> UserPets { get; set; }
    public DbSet<UserSession> UserSessionHistory { get; set; }
    public DbSet<Friendship> Friends { get; set; }
    public DbSet<Decor> Decor { get; set; }
    public DbSet<UserDecor> UserDecor { get; set; }
    public DbSet<MindfulnessTip> MindfulnessTips { get; set; }
    public DbSet<Island> Islands { get; set; }
    public DbSet<UserIsland> UserIslands { get; set; }

    /// <summary>
    /// If the database isn't created, create it.
    /// If the tables aren't created, create them.
    /// </summary>
    public FocusContext(DbContextOptions<FocusContext> options) : base(options)
    {
        if (Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator dbCreator)
        {
            if (!dbCreator.CanConnect() || !dbCreator.HasTables())
            {
                Database.Migrate();
            }
        }
    }
}
