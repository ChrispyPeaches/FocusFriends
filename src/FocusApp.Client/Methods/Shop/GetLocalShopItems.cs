using FocusApp.Shared.Data;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.Shop;
public class GetLocalShopItems
{
    public class Query : IRequest<List<ShopItem>> { }
    public class Handler : IRequestHandler<Query, List<ShopItem>>
    {
        FocusAppContext _localContext;
        public Handler(FocusAppContext localContext)
        {
            _localContext = localContext;
        }

        public async Task<List<ShopItem>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<ShopItem> pets = await _localContext.Pets
                .Where(p => p.Price > 0)
                .Select(p => new ShopItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageSource = p.Image,
                    Type = ShopItemType.Pets,
                }).ToListAsync();

            List<ShopItem> islands = await _localContext.Islands
                .Where(p => p.Price > 0)
                .Select(i => new ShopItem
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    ImageSource = i.Image,
                    Type = ShopItemType.Islands
                }).ToListAsync();

            List<ShopItem> decor = await _localContext.Decor
                .Select(d => new ShopItem
                {
                    Id = d.Id,
                    Name = d.Name,
                    Price = d.Price,
                    ImageSource = d.Image,
                    Type = ShopItemType.Decor
                }).ToListAsync();


            return pets.Concat(decor).Concat(islands).OrderBy(item => item.Price).ToList();
        }
    }
}