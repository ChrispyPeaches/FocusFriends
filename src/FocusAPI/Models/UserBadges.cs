using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserBadges
{
    [Key]
    public Guid Id { get; set; }

    public User User { get; set; } = null!;

    public Badge Badge { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}
