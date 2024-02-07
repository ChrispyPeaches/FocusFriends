using FocusAppShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Resources;

namespace FocusAppShared.Data;

public class BaseFocusAppContext : DbContext
{
    #region Tables

    public DbSet<User> Users { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<UserBadge> UserBadges { get; set; }
    public DbSet<UserPet> UserPets { get; set; }
    public DbSet<UserSession> UserSessionHistory { get; set; }
    public DbSet<Friendship> Friends { get; set; }

    #endregion

    private static bool Initialized { get; set; }

    public BaseFocusAppContext(DbContextOptions options) : base(options)
    {
        Initialize();
    }

    public BaseFocusAppContext()
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

            Database.EnsureCreated();
            Database.Migrate();
        }
    }

    /// <summary>
    /// Configuration for creating migrations.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Filename={Path.Combine("../", Consts.DatabaseFileName)}");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}
