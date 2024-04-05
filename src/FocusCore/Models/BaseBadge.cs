using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Badges")]
public class BaseBadge
{
    [Key]
    public Guid Id { get; set; }

    public required string Name { get; set; } = null!;

    public required string FilePath { get; set; } = null!;
}
