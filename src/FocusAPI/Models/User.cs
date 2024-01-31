using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusAPI.Models;

public class User
{
	[Key]
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public DateTimeOffset DateCreated { get; set; }

	public int Balance { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public string? Pronouns { get; set; }

	public byte[]? ProfilePicture { get; set; }

	public ICollection<UserFriends>? Friends { get; set; }

	public ICollection<UserPets>? Pets { get; set; }

	public ICollection<UserBadges>? Badges { get; set; }

	public ICollection<UserSessionHistory>? UserSessions { get; set; }
}
