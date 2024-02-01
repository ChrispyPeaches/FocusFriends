using FocusApp.Clients;
using FocusCore.Models.User;
using FocusCore.Queries.User;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Helpers
{
    public class APIService
    {
        IAPIClient _client { get; set; }
        public APIService()
        {
            // TODO: Pull api URL from config
            _client = RestService.For<IAPIClient>("http://10.0.2.2:5223");
        }

        public async Task<UserModel> GetUser(GetUserQuery query)
        {
            UserModel user = await _client.GetUser(query);
            return user;
        }
    }
}