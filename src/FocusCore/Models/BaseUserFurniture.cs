using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Models;

[Table("UserFurniture")]
[PrimaryKey(nameof(UserId), nameof(FurnitureId))]
public class BaseUserFurniture
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser User { get; set; } = null!;

    [ForeignKey(nameof(Furniture))]
    public Guid FurnitureId { get; set; }

    public BaseFurniture Furniture { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}