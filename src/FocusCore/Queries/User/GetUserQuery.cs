using FocusCore.Models.User;
using MediatR;

namespace FocusCore.Queries.User;
public class GetUserQuery : IRequest<UserModel>
{
    public Guid Id { get; set; }
}

