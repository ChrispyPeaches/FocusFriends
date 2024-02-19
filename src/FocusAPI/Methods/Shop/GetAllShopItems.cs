using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;

namespace FocusApi.Methods.Shop;
public class GetAllShopItems
{
    public class Handler : IRequestHandler<GetAllShopItemsQuery, List<BasePet>>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<List<BasePet>> Handle(GetAllShopItemsQuery query, CancellationToken cancellationToken)
        {
            //List<Pet> pets = _context.Pets.ToList();
            List<BasePet> pets = _context.Pets.OfType<BasePet>().ToList();
            return pets;
        }
    }
}