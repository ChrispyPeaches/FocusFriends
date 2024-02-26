using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;

namespace FocusApi.Methods.Shop;
public class GetAllShopItems
{
    public class Handler : IRequestHandler<GetAllShopItemsQuery, List<ShopItem>>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<List<ShopItem>> Handle(GetAllShopItemsQuery query, CancellationToken cancellationToken)
        {
            List<BasePet> pets = _context.Pets.OfType<BasePet>().ToList();
            List<BaseFurniture> furniture = _context.Furniture.OfType<BaseFurniture>().ToList();
            List<BaseSound> sounds = _context.Sounds.OfType<BaseSound>().ToList();

            var petItems = pets.Select(p => new ShopItem
            {
                Name = p.Name,
                Price = p.Price,
                ImageSource = p.Image,
                Type = ShopItemType.Pets
            })
            .ToList();

            var furnitureItems = furniture.Select(f => new ShopItem
            {
                Name = f.Name,
                Price = f.Price,
                ImageSource = f.Image,
                Type = ShopItemType.Furniture
            })
            .ToList();

            var soundItems = sounds.Select(s => new ShopItem
            {
                Name = s.Name,
                Price = s.Price,
                ImageSource = s.Image,
                Type = ShopItemType.Sounds
            }).ToList();

            return petItems.Concat(furnitureItems).Concat(soundItems).ToList();
        }
    }
}