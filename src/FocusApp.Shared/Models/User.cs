using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusApp.Shared.Models;

public class User : FocusCore.Models.BaseUser
{
	public new DateTime DateCreated { get; set; }

	[InverseProperty(nameof(Friendship.User))]
	public new ICollection<Friendship>? Inviters { get; set; } = new List<Friendship>();

    [InverseProperty(nameof(Friendship.Friend))]
    public new ICollection<Friendship>? Invitees { get; set; } = new List<Friendship>();

	public new ICollection<UserPet>? Pets { get; set; } = new List<UserPet>();

    public new ICollection<UserDecor>? Decor { get; set; } = new List<UserDecor>();

    public new ICollection<UserBadge>? Badges { get; set; } = new List<UserBadge>();

    public new ICollection<UserIsland>? Islands { get; set; } = new List<UserIsland>();

	public new ICollection<UserSession>? UserSessions { get; set; } = new List<UserSession>();

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Island? SelectedIsland { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Pet? SelectedPet { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Decor? SelectedDecor { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public new Badge? SelectedBadge { get; set; }
}
