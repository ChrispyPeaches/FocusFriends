using MediatR;

namespace FocusCore.Commands.User;
public class AddUserDecorCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid DecorId { get; set; }
    public int UpdatedBalance { get; set; }
}