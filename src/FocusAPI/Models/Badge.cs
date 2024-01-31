using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

[Table("Badges")]
public class Badge
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string FilePath { get; set; } = null!;
}
