using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries.User;
public class GetUserQuery : IRequest<BaseUser>
{
    public Guid Id { get; set; }
    public string Auth0Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}