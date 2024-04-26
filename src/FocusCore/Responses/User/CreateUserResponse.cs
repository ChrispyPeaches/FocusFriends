namespace FocusCore.Responses.User;

public class CreateUserResponse
{
    public UserDto? User { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public int Balance { get; set; }
    public List<Guid>? UserIslandIds { get; set; }
    public List<Guid>? UserPetIds { get; set; }
    public Guid? SelectedIslandId { get; set; }
    public Guid? SelectedPetId { get; set; }
    // Note: This offset is UTC
    public DateTimeOffset DateCreated { get; set; }
}
