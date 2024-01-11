using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries;
public class GetShopDataQuery : IRequest<UserModel>
{
    public Guid Id { get; set; }
}

