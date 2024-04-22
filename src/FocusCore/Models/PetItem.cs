using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class PetItem
    {
        public Guid PetId { get; set; }
        public string PetName { get; set; }
        public byte[] PetsProfilePicture { get; set; }

        // Bool to determine whether to display checkmark
        public bool isSelected { get; set; }
    }
}
