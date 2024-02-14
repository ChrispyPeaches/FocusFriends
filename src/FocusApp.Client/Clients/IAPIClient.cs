using FocusApp.Shared.Models;
using FocusCore.Models;
using FocusCore.Queries.User;
using Refit;

namespace FocusApp.Client.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<FocusApp.Shared.Models.User> GetUser(GetUserQuery query);
}