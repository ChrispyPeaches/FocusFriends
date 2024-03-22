using Microsoft.EntityFrameworkCore;

namespace FocusApp.Shared.Models;
public class UserSound : FocusCore.Models.BaseUserSound
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Sound? Sound { get; set; }

    public new DateTime DateAcquired { get; set; }
}