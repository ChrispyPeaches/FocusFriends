using FocusCore.Models.User;
using MediatR;

namespace FocusCore.Queries.Shop;
public class GetShopDataQuery : IRequest<UserModel>
{
    public Guid Id { get; set; }
}

