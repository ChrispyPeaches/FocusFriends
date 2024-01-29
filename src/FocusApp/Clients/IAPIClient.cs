using FocusCore.Models.User;
using FocusCore.Queries.User;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Clients;
public interface IAPIClient
{
    [Get("/User")]
    Task<UserModel> GetUser();
}