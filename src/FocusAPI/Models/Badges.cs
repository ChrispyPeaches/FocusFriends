using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class Badges
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string FilePath { get; set; } = null!;
}
