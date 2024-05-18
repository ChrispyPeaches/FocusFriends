using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusAPI.Data;
using FocusAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Tests.Helpers;
public class TestAPIContext : DbContext, IFocusAPIContext
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

    public TestAPIContext(DbContextOptions<TestAPIContext> options) : base(options) { }
}