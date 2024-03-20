using FocusApp.Shared.Models;
using FocusCore.Commands.User;
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

    [Post("/User/Pet")]
    Task AddUserPet(AddUserPetCommand command);

    [Post("/User/Furniture")]
    Task AddUserFurniture(AddUserFurnitureCommand command);

    [Post("/User/Sound")]
    Task AddUserSound(AddUserSoundCommand command);
}