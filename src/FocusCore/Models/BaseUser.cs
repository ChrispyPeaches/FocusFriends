﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FocusCore.Models;

[Table("Users")]
public class BaseUser
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
    public string? Pronouns { get; set; }

	public byte[]? ProfilePicture { get; set; }

	public ICollection<BaseFriendship>? Inviters { get; set; } = new List<BaseFriendship>();

	public ICollection<BaseFriendship>? Invitees { get; set; } = new List<BaseFriendship>();

    public ICollection<BaseUserPet>? Pets { get; set; } = new List<BaseUserPet>();

    public ICollection<BaseUserDecor>? Decor { get; set; } = new List<BaseUserDecor>();

    public ICollection<BaseUserBadge>? Badges { get; set; } = new List<BaseUserBadge>();

    public ICollection<BaseUserIsland>? Islands { get; set; } = new List<BaseUserIsland>();

    public ICollection<BaseUserSession>? UserSessions { get; set; } = new List<BaseUserSession>();

    [ForeignKey(nameof(SelectedIsland))]
    public Guid? SelectedIslandId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public BaseIsland? SelectedIsland { get; set; }

    [ForeignKey(nameof(SelectedPet))]
    public Guid? SelectedPetId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public BasePet? SelectedPet { get; set; }

    [ForeignKey(nameof(SelectedDecor))]
    public Guid? SelectedDecorId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public BaseDecor? SelectedDecor { get; set; }

    [ForeignKey(nameof(SelectedBadge))]
    public Guid? SelectedBadgeId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public BaseBadge? SelectedBadge { get; set; }
}