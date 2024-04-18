﻿using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;

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
            List<ShopItem> pets = _apiContext.Pets.OfType<BasePet>().Select(p => new ShopItem
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSource = p.Image,
                Type = ShopItemType.Pets,
            }).ToList();

            List<ShopItem> decor = _apiContext.Decor.OfType<BaseDecor>().Select(f => new ShopItem
            {
                Id = f.Id,
                Name = f.Name,
                Price = f.Price,
                ImageSource = f.Image,
                Type = ShopItemType.Decor
            }).ToList();

            /*List<ShopItem> sounds = _apiContext.Sounds.OfType<BaseSound>().Select(s => new ShopItem
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                ImageSource = s.Image,
                Type = ShopItemType.Sounds
            }).ToList();*/

            return pets.Concat(decor)/*.Concat(sounds)*/.ToList();
        }
    }
}