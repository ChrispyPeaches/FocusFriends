using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class UserPets
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Pet Pet { get; set; } = null!;

    public Guid PetId { get; set; }

    public DateTimeOffset DateAcquired { get; set; }
}
