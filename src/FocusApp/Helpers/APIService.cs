using FocusApp.Clients;
using FocusCore.Queries.User;
using FocusCore.Models.User;
using Refit;

namespace FocusApp.Helpers;
public class APIService
{
    //public IAPIClient _client;
    public APIService()
    {
        //_client = RestService.For<IAPIClient>("http://10.0.2.2:5223/");
    }

    public async Task<UserModel> GetUser(GetUserQuery query)
    {
        //var user = await _client.GetUser(/*query*/);
        return new UserModel();
    }
}