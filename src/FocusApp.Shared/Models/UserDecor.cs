using Microsoft.EntityFrameworkCore;

namespace FocusApp.Shared.Models;
public class UserDecor : FocusCore.Models.BaseUserDecor
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Decor? Decor { get; set; }

    public new DateTime DateAcquired { get; set; }
}