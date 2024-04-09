using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Models
{
    public class FriendListModel
    {
        public string FriendUserName { get; set; }
        public string FriendEmail { get; set; }
        public byte[] FriendProfilePicture { get; set; }
    }
}
