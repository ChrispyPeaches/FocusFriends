using Microsoft.EntityFrameworkCore;

namespace FocusApp.Models;

public class UserBadge : FocusCore.Models.BaseUserBadge
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Badge? Badge { get; set; }
}
