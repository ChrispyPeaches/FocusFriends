using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Pets")]
public class BasePet
{
    [Key]
    public Guid Id { get; set; }

    public required int Price { get; set; }

    public required string Name { get; set; } = null!;
    public byte[] Image { get; set; }
}
