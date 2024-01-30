using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class UserSessionHistory
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset SessionStartTime { get; set; }

    public DateTimeOffset SessionEndTime { get; set; }

    public int CurrencyEarned { get; set; }
}
