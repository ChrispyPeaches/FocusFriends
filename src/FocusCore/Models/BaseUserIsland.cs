using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("UserIslands")]
[PrimaryKey(nameof(UserId), nameof(IslandId))]
public class BaseUserIsland
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser? User { get; set; }

    [ForeignKey(nameof(Island))]
    public Guid IslandId { get; set; }

    public BaseIsland? Island { get; set; }

    public DateTimeOffset DateAcquired { get; set; }
}
