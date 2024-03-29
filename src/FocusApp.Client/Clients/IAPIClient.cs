using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.Leaderboard;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Sync;
using FocusCore.Models;
using Refit;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<User> GetUserByAuth0Id(GetUserQuery query);

    [Get("/Shop")]
    Task<List<Shared.Models.ShopItem>> GetAllShopItems(
        GetAllShopItemsQuery query,
        CancellationToken cancellationToken);

    [Get("/Leaderboard")]
    Task<List<LeaderboardDto>> GetDailyLeaderboard(
        GetDailyLeaderboardQuery query,
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

}