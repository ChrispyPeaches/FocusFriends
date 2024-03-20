using FocusApp.Shared.Models;
using FocusCore.Queries.Shop;
using FocusCore.Queries.User;
using Refit;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<User> GetUserByAuth0Id(GetUserQuery query);

    [Get("/Shop")]
    Task<List<ShopItem>> GetAllShopItems(GetAllShopItemsQuery query);
}