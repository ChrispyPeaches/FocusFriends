using FocusApp.Shared.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Social;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Social;
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

    [Post("/User/Pet")]
    Task AddUserPet(AddUserPetCommand command);

    [Post("/User/Furniture")]
    Task AddUserFurniture(AddUserFurnitureCommand command);

    [Post("/User/Sound")]
    Task AddUserSound(AddUserSoundCommand command);

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
    Task<SyncMindfulnessTipsResponse> SyncMindfulnessTips(
        [Body] SyncMindfulnessTipsQuery query,
        CancellationToken cancellationToken);

    [Post("/Sync/InitialData")]
    Task<SyncInitialDataResponse> SyncInitialData(
        [Body] SyncInitialDataQuery query,
        CancellationToken cancellationToken);

    #endregion

    [Get("/Social/Friend")]
    Task<List<FriendListModel>> GetAllFriends(GetAllFriendsQuery query, CancellationToken cancellationToken = default);

    [Get("/Social/FriendRequest")]
    Task<List<FriendRequest>> GetAllFriendRequests(GetAllFriendRequestsQuery query, CancellationToken cancellationToken = default);

    [Post("/Social/FriendRequest")]
    Task<CreateFriendRequestResponse> CreateFriendRequest(CreateFriendRequestCommand command);

    [Put("/Social/FriendRequest")]
    Task AcceptFriendRequest([Body] AcceptFriendRequestCommand command);

    [Delete("/Social/FriendRequest")]
    Task CancelFriendRequest([Body] CancelFriendRequestCommand command);

}