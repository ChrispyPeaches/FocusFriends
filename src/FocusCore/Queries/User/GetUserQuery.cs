using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries.User;
public class GetUserQuery : IRequest<BaseUser>
{
    public Guid Id { get; set; }
}

