using FocusCore.Responses;
using MediatR;

namespace FocusCore.Commands.User;

public class EditUserSelectedBadgeCommand : IRequest<MediatrResult>
{
    public Guid? UserId { get; set; }
    public Guid? BadgeId { get; set; }
}