using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class User : FocusCore.Models.BaseUser
{
	[InverseProperty(nameof(UserFriend.User))]
	public new ICollection<UserFriend>? Inviters { get; set; }

    [InverseProperty(nameof(UserFriend.Friend))]
    public new ICollection<UserFriend>? Invitees { get; set; }

	public new ICollection<UserPet>? Pets { get; set; }

	public new ICollection<UserBadge>? Badges { get; set; }

	public new ICollection<UserSession>? UserSessions { get; set; }
}
