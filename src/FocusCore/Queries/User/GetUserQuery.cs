using FocusCore.Responses;
using FocusCore.Responses.User;
using MediatR;

namespace FocusCore.Queries.User;
public class GetUserQuery : IRequest<MediatrResultWrapper<GetUserResponse>>
{
    public string? Auth0Id { get; set; }
}