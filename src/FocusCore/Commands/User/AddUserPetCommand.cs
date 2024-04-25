using MediatR;

namespace FocusCore.Commands.User;
public class AddUserPetCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid PetId { get; set; }
    public int UpdatedBalance { get; set; }
}