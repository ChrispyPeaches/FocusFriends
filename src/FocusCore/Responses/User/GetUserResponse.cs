using FocusCore.Models;

namespace FocusCore.Responses.User;

public class GetUserResponse
{
    public BaseUser? User { get; set; }
    public List<Guid>? UserIslandIds { get; set; }
    public List<Guid>? UserPetIds { get; set; }
    public Guid? SelectedIslandId { get; set; }
    public Guid? SelectedPetId { get; set; }
}