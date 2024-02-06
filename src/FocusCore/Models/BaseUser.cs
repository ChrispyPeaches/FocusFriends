using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

[Table("Users")]
public abstract class BaseUser
{
	[Key]
	public Guid Id { get; set; }

	public string UserName { get; set; } = null!;

	public string Email { get; set; } = null!;

	public DateTimeOffset DateCreated { get; set; }

	public int Balance { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public string? Pronouns { get; set; }

	public byte[]? ProfilePicture { get; set; }

	public ICollection<BaseFriendship>? Inviters { get; set; }

	public virtual ICollection<BaseFriendship>? Invitees { get; set; }

	public virtual ICollection<BaseUserPet>? Pets { get; set; }

	public virtual ICollection<BaseUserBadge>? Badges { get; set; }

	public ICollection<BaseUserSession>? UserSessions { get; set; }
}
