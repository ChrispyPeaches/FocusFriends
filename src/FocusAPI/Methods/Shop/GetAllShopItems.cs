using FocusAPI.Data;
using FocusAPI.Models;
using FocusAPI.Queries.Shop;
using MediatR;

namespace FocusApi.Methods.Shop;
public class GetAllShopItems
{
    public class Handler : IRequestHandler<GetAllShopItemsQuery, List<Pet>>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<List<Pet>> Handle(GetAllShopItemsQuery query, CancellationToken cancellationToken)
        {
            List<Pet> pets = _context.Pets.ToList();
            return pets;
        }
    }
}