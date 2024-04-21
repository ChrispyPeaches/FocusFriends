using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<Decor> Decor { get; set; }
    public DbSet<UserDecor> UserDecor { get; set; }
    public DbSet<MindfulnessTip> MindfulnessTips { get; set; }
    public DbSet<Island> Islands { get; set; }
    public DbSet<UserIsland> UserIslands { get; set; }

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
    public DbSet<Decor> Decor { get; set; }
    public DbSet<UserDecor> UserDecor { get; set; }
    public DbSet<MindfulnessTip> MindfulnessTips { get; set; }
    public DbSet<Island> Islands { get; set; }
    public DbSet<UserIsland> UserIslands { get; set; }

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
