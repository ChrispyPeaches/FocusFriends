using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class UserPets
{
    [Key]
    public Guid Id { get; set; }

    public Users UserId { get; set; } = null!;

    public ICollection<Pets>? Pets { get; set; }

    public DateTimeOffset DateAcquired { get; set; }
}
