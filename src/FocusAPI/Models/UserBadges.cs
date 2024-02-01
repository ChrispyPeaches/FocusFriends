using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserBadges
{
    public User User { get; set; } = null!;

    public Guid UserId { get; set; }

    public Badge Badge { get; set; } = null!;

    public Guid BadgeId { get; set; }

    public DateTimeOffset DateAcquired { get; set; }
}
