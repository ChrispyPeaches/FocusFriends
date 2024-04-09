using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Models;

[Table("UserSounds")]
[PrimaryKey(nameof(UserId), nameof(SoundId))]
public class BaseUserSound
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser User { get; set; } = null!;

    [ForeignKey(nameof(Sound))]
    public Guid SoundId { get; set; }

    public BaseSound Sound { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}

