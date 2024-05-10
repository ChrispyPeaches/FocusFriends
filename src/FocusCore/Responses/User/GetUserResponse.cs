using FocusCore.Models;

namespace FocusCore.Responses.User;

public class GetUserResponse
{
    public BaseUser? User { get; set; }
    public List<Guid> UserIslandIds { get; set; } = new List<Guid>();
    public List<Guid> UserPetIds { get; set; } = new List<Guid>();
    public List<Guid> UserDecorIds { get; set; } = new List<Guid>();
    public List<Guid> UserBadgeIds { get; set; } = new List<Guid>();
}