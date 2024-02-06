using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Pets")]
public abstract class BasePet
{
    [Key]
    public Guid Id { get; set; }

    public int Price { get; set; }

    public string Name { get; set; } = null!;
}
