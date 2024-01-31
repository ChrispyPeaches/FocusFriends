using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class UserFriends
{
    [Key]
    public Guid Id { get; set; }

    public Guid PrimaryUserId { get; set; }

    public ICollection<Users>? FriendIds { get; set; }

}
