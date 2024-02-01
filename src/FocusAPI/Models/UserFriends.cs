using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserFriends
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid FriendId { get; set; }
    public User Friend { get; set; } = null!;
    public int Status { get; set; }
}
