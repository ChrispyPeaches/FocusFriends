using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserSessionHistory
{
    [Key]
    public Guid Id { get; set; }

    public User User { get; set; } = null!;

    public DateTimeOffset SessionStartTime { get; set; }

    public DateTimeOffset SessionEndTime { get; set; }

    public int CurrencyEarned { get; set; }
}
