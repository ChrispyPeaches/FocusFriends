using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Models;

public class UserFriend : FocusCore.Models.BaseUserFriend
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? Friend { get; set; }
}
