using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class DecorItem
    {
        public Guid DecorId { get; set; }
        public string DecorName { get; set; }
        public byte[] DecorPicture { get; set; }

        // Bool to determine whether to display checkmark
        public bool isSelected { get; set; }
    }
}
