using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class UserBadges
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public ICollection<Badges>? Badges { get; set; }
}
