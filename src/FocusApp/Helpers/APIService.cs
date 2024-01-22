using FocusApp.Clients;
using FocusCore.Queries.User;
using FocusCore.Models.User;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Helpers;
public class APIService
{
    public IAPIClient _client;
    public APIService()
    {
        _client = RestService.For<IAPIClient>("http://localhost:5223/");
    }

    public async Task<UserModel> GetUser(GetUserQuery query)
    {
        var user = await _client.GetUser(query);
        return user;
    }
}