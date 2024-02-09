using Microsoft.EntityFrameworkCore;

namespace FocusAppShared.Models;

public class UserPet : FocusCore.Models.BaseUserPet
{
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new User? User { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Pet? Pet { get; set; }

    public new DateTime DateAcquired { get; set; }
}
