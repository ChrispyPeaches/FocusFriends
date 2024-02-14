using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("UserBadges")]
[PrimaryKey(nameof(UserId), nameof(BadgeId))]
public abstract class BaseUserBadge
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser? User { get; set; }

    [ForeignKey(nameof(Badge))]
    public Guid BadgeId { get; set; }

    public BaseBadge? Badge { get; set; }

    public DateTimeOffset DateAcquired { get; set; }
}
