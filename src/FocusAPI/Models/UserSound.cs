using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Models;
public class UserSound : FocusCore.Models.BaseUserSound
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Sound? Sound { get; set; }
}