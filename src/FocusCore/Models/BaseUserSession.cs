using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("UserSessionHistory")]
public abstract class BaseUserSession
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    
    public BaseUser? User { get; set; }

    public DateTimeOffset SessionStartTime { get; set; }

    public DateTimeOffset SessionEndTime { get; set; }

    public int CurrencyEarned { get; set; }
}
