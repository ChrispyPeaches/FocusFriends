using FocusApp.Shared.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

Console.WriteLine($"Use 'Add-Migration \"<migration name>\" -Context {typeof(FocusAppContext).FullName} -Project {typeof(FocusAppContext).Assembly.GetName().Name} -StartupProject {typeof(Program).Assembly.GetName().Name}' to add a new migration");

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