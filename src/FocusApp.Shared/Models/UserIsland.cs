using Microsoft.EntityFrameworkCore;

namespace FocusApp.Shared.Models;
public class UserIsland : FocusCore.Models.BaseUserIsland
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Island? Island { get; set; }

    public new DateTime DateAcquired { get; set; }
}