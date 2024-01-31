using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserPets
{
    [Key]
    public Guid Id { get; set; }

    public User User { get; set; } = null!;

    public Pet Pet { get; set; } = null!;

    public DateTimeOffset DateAcquired { get; set; }
}
