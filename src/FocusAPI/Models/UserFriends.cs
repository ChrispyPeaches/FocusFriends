using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserFriends
{
    [Key]
    public Guid Id { get; set; }

    public User User { get; set; } = null!;

    public User Friend { get; set; } = null!;

}
