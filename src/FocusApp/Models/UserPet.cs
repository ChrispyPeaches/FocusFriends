using Microsoft.EntityFrameworkCore;

namespace FocusApp.Models;

public class UserPet : FocusCore.Models.BaseUserPet
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Pet? Pet { get; set; }
}
