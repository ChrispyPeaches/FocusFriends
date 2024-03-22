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
    public DbSet<Furniture> Furniture { get; set; }
    public DbSet<Sound> Sounds { get; set; }
    public DbSet<UserFurniture> UserFurniture { get; set; }
    public DbSet<UserSound> UserSounds { get; set; }
    public DbSet<MindfulnessTip> MindfulnessTips { get; set; }

    /// <summary>
    /// If the database isn't created, create it.
    /// If the tables aren't created, create them.
    /// </summary>
    public FocusContext(DbContextOptions<FocusContext> options) : base(options)
    {
        if (Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator dbCreator)
        {
            if (!dbCreator.CanConnect())
            {
                dbCreator.Create();
            }

            if (!dbCreator.HasTables())
            {
                dbCreator.CreateTables();
            }
        }
    }
}
