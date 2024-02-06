using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Badges")]
public abstract class BaseBadge
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string FilePath { get; set; } = null!;
}
