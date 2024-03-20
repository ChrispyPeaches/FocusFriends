using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Models;

[Table("UserFurniture")]
[PrimaryKey(nameof(UserId), nameof(FurnitureId))]
public abstract class BaseUserFurniture
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser User { get; set; } = null!;

    [ForeignKey(nameof(Furniture))]
    public Guid FurnitureId { get; set; }

    public BaseFurniture Furniture { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}