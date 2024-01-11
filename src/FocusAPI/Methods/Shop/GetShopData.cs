using FocusCore.Models;
using FocusCore.Queries;
using MediatR;

namespace FocusApi.Methods.Shop;
public class GetShopData
{
    public class Handler : IRequestHandler<GetShopDataQuery, UserModel>
    {
        // TODO: Add db client references
        public Handler() { }

        public async Task<UserModel> Handle(GetShopDataQuery query, CancellationToken cancellationToken)
        {
            return new UserModel { Id = Guid.NewGuid(), Name = "Gunter" };
        }
    }
}