using FocusCore.Responses;
using MediatR;

namespace FocusCore.Commands.User;
public class AddUserBadgeCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
}