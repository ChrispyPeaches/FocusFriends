using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("UserFriends")]
[PrimaryKey(nameof(UserId), nameof(FriendId), nameof(Status))]
public abstract class BaseUserFriend
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public BaseUser? User { get; set; }

    [ForeignKey(nameof(Friend))]
    public Guid FriendId { get; set; }

    public BaseUser? Friend { get; set; }
    public int Status { get; set; }
}
