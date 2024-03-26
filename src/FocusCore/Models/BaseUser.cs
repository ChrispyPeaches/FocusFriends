using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FocusCore.Models;

[Table("Users")]
public abstract class BaseUser
{
	[Key]
	public Guid Id { get; set; }
    public string Auth0Id { get; set; }

    [MaxLength(50)]
    public required string UserName { get; set; } = null!;

    [MaxLength(320)]
    public required string Email { get; set; } = null!;

	public DateTimeOffset DateCreated { get; set; }

	public required int Balance { get; set; }

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? Pronouns { get; set; }

	public byte[]? ProfilePicture { get; set; }

	public ICollection<BaseFriendship>? Inviters { get; set; }

	public ICollection<BaseFriendship>? Invitees { get; set; }

    public ICollection<BaseUserPet>? Pets { get; set; } = new List<BaseUserPet>();

    public ICollection<BaseUserFurniture>? Furniture { get; set; } = new List<BaseUserFurniture>();

    public ICollection<BaseUserSound>? Sounds { get; set; } = new List<BaseUserSound>();

    public ICollection<BaseUserBadge>? Badges { get; set; }

    public ICollection<BaseUserIsland>? Islands { get; set; } = new List<BaseUserIsland>();

	public ICollection<BaseUserSession>? UserSessions { get; set; }
}