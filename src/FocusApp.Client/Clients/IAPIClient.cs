using FocusApp.Shared.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Models;
using FocusCore.Queries.Leaderboard;
using FocusCore.Queries.Social;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Social;
using FocusCore.Responses.Leaderboard;
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
    Task<LeaderboardResponse> GetDailyLeaderboard(
        GetDailyLeaderboardQuery query,
        CancellationToken cancellationToken = default);

    [Get("/Leaderboard/Weekly")]
    Task<LeaderboardResponse> GetWeeklyLeaderboard(
        GetWeeklyLeaderboardQuery query,
        CancellationToken cancellationToken = default);

    [Post("/User/Pet")]
    Task AddUserPet(
        AddUserPetCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/Decor")]
    Task AddUserDecor(
        AddUserDecorCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/Island")]
    Task AddUserIsland(
        AddUserIslandCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/Badge")]
    Task AddUserBadge(
        AddUserBadgeCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/AddSession")]
    Task CreateSession(
        CreateSessionCommand command,
        CancellationToken cancellationToken = default);

    [Post("/User/Edit")]
    Task EditUserProfile(
        EditUserProfileCommand command,
        CancellationToken cancellationToken = default);
     
    #endregion

    #region Social
    [Get("/Social/AllFriends")]
    Task<List<FriendListModel>> GetAllFriends(GetAllFriendsQuery query, CancellationToken cancellationToken = default);

    [Get("/Social/AllFriendRequests")]
    Task<List<FriendRequest>> GetAllFriendRequests(GetAllFriendRequestsQuery query, CancellationToken cancellationToken = default);

    [Post("/Social/FriendRequest")]
    Task<ApiResponse<CreateFriendRequestResponse>> CreateFriendRequest(CreateFriendRequestCommand command);

    [Put("/Social/FriendRequest")]
    Task AcceptFriendRequest([Body] AcceptFriendRequestCommand command);

    [Delete("/Social/FriendRequest")]
    Task CancelFriendRequest([Body] CancelFriendRequestCommand command);

    #endregion

    #region Sync

    [Post("/Sync/MindfulnessTips")]
    Task<SyncItemResponse<BaseMindfulnessTip>> SyncMindfulnessTips(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken = default);

    [Post("/Sync/Badges")]
    Task<SyncItemResponse<BaseBadge>> SyncBadges(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken = default);

    [Post("/Sync/Pets")]
    Task<SyncItemResponse<BasePet>> SyncPets(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken = default);

    [Post("/Sync/Decor")]
    Task<SyncItemResponse<BaseDecor>> SyncDecor(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken = default);

    [Post("/Sync/Islands")]
    Task<SyncItemResponse<BaseIsland>> SyncIslands(
        [Body] SyncItemsQuery query,
        CancellationToken cancellationToken = default);

    [Post("/Sync/InitialData")]
    Task<SyncInitialDataResponse> SyncInitialData(
        [Body] SyncInitialDataQuery query,
        CancellationToken cancellationToken = default);

    #endregion
}