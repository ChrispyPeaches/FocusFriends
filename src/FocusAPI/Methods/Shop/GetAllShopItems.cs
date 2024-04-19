using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.Shop;
public class GetAllShopItems
{
    public class Handler : IRequestHandler<GetAllShopItemsQuery, List<ShopItem>>
    {
        FocusAPIContext _apiContext;
        public Handler(FocusAPIContext apiContext)
        {
            _apiContext = apiContext;
        }

        public async Task<List<ShopItem>> Handle(GetAllShopItemsQuery query, CancellationToken cancellationToken)
        {
            List<ShopItem> pets = await _apiContext.Pets
                .Where(p => p.Price > 0)
                .Select(p => new ShopItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageSource = p.Image,
                    Type = ShopItemType.Pets,
                }).ToListAsync();

            List<ShopItem> decor = await _apiContext.Decor
                .Select(f => new ShopItem
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    ImageSource = f.Image,
                    Type = ShopItemType.Decor
                }).ToListAsync();

            List<ShopItem> islands = await _apiContext.Islands
                .Where(i => i.Price > 0)
                .Select(s => new ShopItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    ImageSource = s.Image,
                    Type = ShopItemType.Islands
                }).ToListAsync();

            return pets.Concat(decor).Concat(islands).ToList();
        }
    }
}