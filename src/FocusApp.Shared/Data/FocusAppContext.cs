using System.Diagnostics;
using System.Reflection;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApp.Shared.Data;

public interface IFocusAppContext
{
    #region Tables

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

    #endregion

    Task SaveChangesAsync();
}

public class FocusAppContext : DbContext, IFocusAppContext
{
    #region Tables

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

    #endregion

    private static bool Initialized { get; set; }

    public FocusAppContext(DbContextOptions options) : base(options)
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!Initialized)
        {
            Initialized = true;

            // required to initialize SQLite on some platforms
            SQLitePCL.Batteries_V2.Init();

            Database.Migrate();
        }
    }

    public Task SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}
