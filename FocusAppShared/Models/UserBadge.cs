using Microsoft.EntityFrameworkCore;

namespace FocusAppShared.Models;

public class UserBadge : FocusCore.Models.BaseUserBadge
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Badge? Badge { get; set; }

    public new DateTime DateAcquired { get; set; }
}
