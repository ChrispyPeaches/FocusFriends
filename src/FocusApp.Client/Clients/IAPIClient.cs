using FocusApp.Shared.Models;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Sync;
using FocusCore.Queries.User;
using FocusCore.Responses.Sync;
using Refit;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<User> GetUser(GetUserQuery query);

    [Get("/Shop")]
    Task<List<ShopItem>> GetAllShopItems(GetAllShopItemsQuery query, CancellationToken cancellationToken);

    [Post("/Sync/MindfulnessTips")]
    Task<SyncMindfulnessTipsResponse> SyncMindfulnessTips(
        [Body] SyncMindfulnessTipsQuery query,
        CancellationToken cancellationToken);
}