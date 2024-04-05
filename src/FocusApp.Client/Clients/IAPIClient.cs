using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.Leaderboard;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Sync;
using FocusCore.Responses.User;
using Refit;

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
    Task<List<FocusCore.Models.LeaderboardDto>> GetDailyLeaderboard(
        GetDailyLeaderboardQuery query,
        CancellationToken cancellationToken);

    [Get("/Leaderboard/Weekly")]
    Task<List<FocusCore.Models.LeaderboardDto>> GetWeeklyLeaderboard(
        GetWeeklyLeaderboardQuery query,
        CancellationToken cancellationToken);

    [Post("/User/Pet")]
    Task AddUserPet(AddUserPetCommand command);

    [Post("/User/Furniture")]
    Task AddUserFurniture(AddUserFurnitureCommand command);

    [Post("/User/Sound")]
    Task AddUserSound(AddUserSoundCommand command);

    #endregion

    #region Shop

    [Get("/Shop")]
    Task<List<ShopItem>> GetAllShopItems(
        GetAllShopItemsQuery query,
        CancellationToken cancellationToken);

    #endregion

    #region Sync

    [Post("/Sync/MindfulnessTips")]
    Task<SyncMindfulnessTipsResponse> SyncMindfulnessTips(
        [Body] SyncMindfulnessTipsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/InitialData")]
    Task<SyncInitialDataResponse> SyncInitialData(
        [Body] SyncInitialDataQuery query,
        CancellationToken cancellationToken);

    #endregion
}