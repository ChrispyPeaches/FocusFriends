using FocusCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Data;

public class FocusContext : DbContext
{
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<Badges> Badges { get; set; } = null!;
    public DbSet<Pets> Pets { get; set; } = null!;
    public DbSet<UserBadges> UserBadges { get; set; } = null!;
}
