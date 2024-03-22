using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Models;
public class UserFurniture : FocusCore.Models.BaseUserFurniture
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Furniture? Furniture { get; set; }
}