using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserFriends
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid FriendId { get; set; }
    public User? Friend { get; set; }
    public int Status { get; set; }
}
