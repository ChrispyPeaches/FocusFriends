using MediatR;

namespace FocusCore.Commands.User;
public class AddUserSoundCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid SoundId { get; set; }
    public int UpdatedBalance { get; set; }
}