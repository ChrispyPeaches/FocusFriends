using FocusAppShared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApp.Data;

public class FocusAppContext : BaseFocusAppContext
{
    public FocusAppContext(DbContextOptions<FocusAppContext> options)
        : base(options) { }

    /// <summary>
    /// Configuration for the Maui app's database context.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Filename={Resources.Consts.DatabasePath}");
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }
}
