using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Decor")]
public class BaseDecor : ISyncEntity
{
    [Key]
    public Guid Id { get; set; }

    public required int Price { get; set; }

    public required string Name { get; set; } = null!;
    public byte[] Image { get; set; }
    public int HeightRequest { get; set; }
}
