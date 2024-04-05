using FocusCore.Responses;
using FocusCore.Responses.User;
using MediatR;

namespace FocusCore.Commands.User;

public class CreateUserCommand : IRequest<MediatrResultWrapper<CreateUserResponse>>
{
    public Guid Id { get; set; }
    public string Auth0Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}