using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Friendships")]
[PrimaryKey(nameof(UserId), nameof(FriendId), nameof(Status))]
public abstract class BaseFriendship
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser? User { get; set; }

    [ForeignKey(nameof(Friend))]
    public Guid FriendId { get; set; }

    public BaseUser? Friend { get; set; }

    public required int Status { get; set; }
}
