using System.ComponentModel.DataAnnotations.Schema;
using FocusCore.Models;

namespace FocusApp.Shared.Models;

public class User : FocusCore.Models.BaseUser
{
	public new DateTime DateCreated { get; set; }

	[InverseProperty(nameof(Friendship.User))]
	public new ICollection<Friendship>? Inviters { get; set; }

    [InverseProperty(nameof(Friendship.Friend))]
    public new ICollection<Friendship>? Invitees { get; set; }

	public new ICollection<UserPet>? Pets { get; set; } = new List<UserPet>();

    public new ICollection<UserFurniture>? Furniture { get; set; } = new List<UserFurniture>();

    public new ICollection<UserSound>? Sounds { get; set; } = new List<UserSound>();

    public new ICollection<UserBadge>? Badges { get; set; }

	public new ICollection<UserSession>? UserSessions { get; set; }
}
