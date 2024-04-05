using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("UserPets")]
[PrimaryKey(nameof(UserId), nameof(PetId))]
public class BaseUserPet
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser User { get; set; } = null!;

    [ForeignKey(nameof(Pet))]
    public Guid PetId { get; set; }

    public BasePet Pet { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}
