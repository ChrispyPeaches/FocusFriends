using Microsoft.EntityFrameworkCore;

namespace FocusApp.Shared.Models;
public class UserFurniture : FocusCore.Models.BaseUserFurniture
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Furniture? Furniture { get; set; }

    public new DateTime DateAcquired { get; set; }
}