using FocusCore.Commands.User;
using FocusCore.Models;
using FocusCore.Queries.Leaderboard;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Leaderboard;
using FocusCore.Responses.Sync;
using FocusCore.Responses.User;
using Refit;
using ShopItem = FocusApp.Shared.Models.ShopItem;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    #region User

    [Get("/User/GetUser")]
    Task<ApiResponse<GetUserResponse>> GetUserByAuth0Id(
        GetUserQuery query,
        CancellationToken cancellationToken = default);

    [Post("/User/CreateUser")]
    Task<CreateUserResponse> CreateUser(
        CreateUserCommand command,
        CancellationToken cancellationToken = default);

    [Get("/Leaderboard/Daily")]
    Task<LeaderboardResponse> GetDailyLeaderboard(
        GetDailyLeaderboardQuery query,
        CancellationToken cancellationToken);

    [Get("/Leaderboard/Weekly")]
    Task<LeaderboardResponse> GetWeeklyLeaderboard(
        GetWeeklyLeaderboardQuery query,
        CancellationToken cancellationToken);

    [Post("/User/Pet")]
    Task AddUserPet(AddUserPetCommand command);

    [Post("/User/Decor")]
    Task AddUserDecor(
        AddUserDecorCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/Island")]
    Task AddUserIsland(
        AddUserIslandCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/AddSession")]
    Task CreateSession(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default);

    #endregion

    #region Shop

    [Get("/Shop")]
    Task<List<ShopItem>> GetAllShopItems(
        GetAllShopItemsQuery query,
        CancellationToken cancellationToken);

    #endregion

    #region Sync

    [Post("/Sync/MindfulnessTips")]
    Task<SyncItemResponse<BaseMindfulnessTip>> SyncMindfulnessTips(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/Badges")]
    Task<SyncItemResponse<BaseBadge>> SyncBadges(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/Pets")]
    Task<SyncItemResponse<BasePet>> SyncPets(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/Decor")]
    Task<SyncItemResponse<BaseDecor>> SyncDecor(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/Islands")]
    Task<SyncItemResponse<BaseIsland>> SyncIslands(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/InitialData")]
    Task<SyncInitialDataResponse> SyncInitialData(
        [Body] SyncInitialDataQuery query,
        CancellationToken cancellationToken);

    #endregion
}