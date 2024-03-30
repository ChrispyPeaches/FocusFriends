using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Furniture")]
public class BaseFurniture
{
    [Key]
    public Guid Id { get; set; }

    public required int Price { get; set; }

    public required string Name { get; set; } = null!;
    public byte[] Image { get; set; }
    public int HeightRequest { get; set; }
}
