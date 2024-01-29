using Refit;

namespace FocusApp.Clients;
public interface IJsonPlaceHolderApi
{
    [Get("/posts")]
    Task<ApiPost> GetPost();
}