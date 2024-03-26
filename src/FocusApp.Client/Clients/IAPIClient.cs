using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Sync;
using Refit;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<User> GetUserByAuth0Id(GetUserQuery query);

    [Get("/Shop")]
    Task<List<ShopItem>> GetAllShopItems(
        GetAllShopItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/User/Pet")]
    Task AddUserPet(AddUserPetCommand command);

    [Post("/User/Furniture")]
    Task AddUserFurniture(AddUserFurnitureCommand command);

    [Post("/User/Sound")]
    Task AddUserSound(AddUserSoundCommand command);

    [Post("/Sync/MindfulnessTips")]
    Task<SyncMindfulnessTipsResponse> SyncMindfulnessTips(
        [Body] SyncMindfulnessTipsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/InitialData")]
    Task<SyncInitialDataResponse> SyncInitialData(
        [Body] SyncInitialDataQuery query,
        CancellationToken cancellationToken);
}