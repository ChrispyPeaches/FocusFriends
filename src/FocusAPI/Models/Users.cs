using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusCore.Models;

public class Users
{
	[Key]
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public DateTime DateCreated { get; set; }

	public int Balance { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public string? Pronouns { get; set; }

	public byte[]? ProfilePicture { get; set; }

}
