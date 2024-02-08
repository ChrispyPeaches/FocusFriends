using FocusAppShared;
using FocusAppShared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

Console.WriteLine($"This is the efcore CLI migrations entrypoint. Use 'Add-Migration \"<migration name>\" -Context {nameof(FocusAppShared)}.{nameof(FocusAppShared.Data)}.{nameof(FocusAppContext)} to add a new migration");


// This is required by the Entity Framework CLI tools.
public class DesignFocusAppContext : IDesignTimeDbContextFactory<FocusAppContext>
{
    public FocusAppContext CreateDbContext(string[] args)
    {
        return new FocusAppContext(
            new DbContextOptionsBuilder<FocusAppContext>()
                .UseSqlite("Filename=migration.db")
                .Options);
    }
}