using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

[Table("Pets")]
public class Pet
{
    [Key]
    public Guid Id { get; set; }

    public int Price { get; set; }

    public string Name { get; set; } = null!;
}
