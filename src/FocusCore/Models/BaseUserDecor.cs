using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Models;

[Table("UserDecor")]
[PrimaryKey(nameof(UserId), nameof(DecorId))]
public class BaseUserDecor
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser User { get; set; } = null!;

    [ForeignKey(nameof(Decor))]
    public Guid DecorId { get; set; }

    public BaseDecor Decor { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}