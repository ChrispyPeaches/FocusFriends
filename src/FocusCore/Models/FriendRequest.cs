using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class FriendRequest
    {
        public Guid FriendId { get; set; }
        public string FriendUserName { get; set; }
        public required string FriendEmail { get; set; } = null!;
        public byte[] FriendProfilePicture { get; set; }
        public int FriendStatus { get; set; }

        // True if User is Inviter
        public bool userInitiated { get; set; }
    }
}
