using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class IslandItem
    {
        public Guid IslandId { get; set; }
        public string IslandName { get; set; }
        public byte[] IslandPicture { get; set; }

        // Bool to determine whether to display checkmark
        public bool isSelected { get; set; }
    }
}
