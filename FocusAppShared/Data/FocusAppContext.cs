using System.Diagnostics;
using System.Reflection;
using FocusAppShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusAppShared.Data;

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
            Database.EnsureDeleted();
            // required to initialize SQLite on some platforms
            SQLitePCL.Batteries_V2.Init();

            Database.Migrate();
        }
    }

    public Task SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

#if MIGRATION_PROJECT
    /// <summary>
    /// Configuration for creating migrations.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            var a = Process.GetCurrentProcess().ProcessName;

            Assembly currentlyRunningAssembly = null;
            if (a.Contains("FocusAppTools"))
            {
                currentlyRunningAssembly = Assembly.GetAssembly(typeof(FocusAppShared.Consts));
            }
            else
            {
                currentlyRunningAssembly = Assembly.GetExecutingAssembly();

                // Get the type of MyClass in the current assembly
            }
            string databasePath = "";

            var d = "";

            var b = currentlyRunningAssembly
                .GetTypes()
                .FirstOrDefault(type => type.Name == nameof(Consts));

            var c = b
                .GetProperty(nameof(Consts.DatabasePath))?
                .GetValue(null) as string;
            optionsBuilder.UseSqlite($"Filename={databasePath}");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }
        catch (FileNotFoundException ex)
        {
            throw new FileNotFoundException($"The {nameof(Consts)} class holding the database file name could not be found.", ex);
        }
    }
#endif

}
