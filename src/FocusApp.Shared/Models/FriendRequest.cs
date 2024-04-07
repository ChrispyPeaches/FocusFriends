using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Shared.Models
{
    public class FriendRequest
    {
        public Guid FriendId { get; set; }
        public string FriendUserName { get; set; }
        public required string FriendEmail { get; set; } = null!;
        public byte[] FriendProfilePicture { get; set; }

        // Bool to determine whether to display accept/reject or cancel button
        public bool UserInitiated { get; set; }
    }
}
