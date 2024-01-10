using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries;
public class GetUserDataQuery : IRequest<UserModel>
{
    public Guid Id { get; set; }
}

