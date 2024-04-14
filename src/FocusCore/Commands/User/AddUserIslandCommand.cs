using MediatR;

namespace FocusCore.Commands.User;
public class AddUserIslandCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid IslandId { get; set; }
    public int UpdatedBalance { get; set; }
}