using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class FriendModel
    {
        public string FriendUserName { get; set; }
        public required string Email { get; set; } = null!;
        public byte[] ProfilePicture { get; set; }
    }
}
