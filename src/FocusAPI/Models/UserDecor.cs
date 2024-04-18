using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Models;
public class UserDecor : FocusCore.Models.BaseUserDecor
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Decor? Decor { get; set; }
}