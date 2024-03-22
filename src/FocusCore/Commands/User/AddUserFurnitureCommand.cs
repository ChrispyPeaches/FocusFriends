using MediatR;

namespace FocusCore.Commands.User;
public class AddUserFurnitureCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid FurnitureId { get; set; }
    public int UpdatedBalance { get; set; }
}