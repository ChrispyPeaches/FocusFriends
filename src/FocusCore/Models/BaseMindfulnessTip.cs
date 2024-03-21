using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models
{
    [Table("MindfulnessTips")]
    public class BaseMindfulnessTip
    {
        [Key]
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string SessionRatingLevel { get; set; }
    }
}
