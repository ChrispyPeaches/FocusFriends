using System.ComponentModel.DataAnnotations.Schema;

namespace FocusApp.Shared.Models;

public class User : FocusCore.Models.BaseUser
{
	public new DateTime DateCreated { get; set; }

	[InverseProperty(nameof(Friendship.User))]
	public new ICollection<Friendship>? Inviters { get; set; }

    [InverseProperty(nameof(Friendship.Friend))]
    public new ICollection<Friendship>? Invitees { get; set; }

	public new ICollection<UserPet>? Pets { get; set; }

	public new ICollection<UserBadge>? Badges { get; set; }

	public new ICollection<UserSession>? UserSessions { get; set; }
}
