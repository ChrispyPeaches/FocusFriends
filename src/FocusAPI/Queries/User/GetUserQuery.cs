using FocusAPI.Models;
using MediatR;

namespace FocusAPI.Queries.User;
public class GetUserQuery : IRequest<Models.User>
{
    public Guid Id { get; set; }
}

