using FocusCore.Queries.User;
using FocusAPI.Models;
using FocusCore.Models;
using MediatR;

namespace FocusApi.Methods.User;
public class GetUser
{
    public class Handler : IRequestHandler<GetUserQuery, BaseUser>
    {
        // TODO: Add db client references
        public Handler() { }

        public async Task<BaseUser> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var user = new FocusAPI.Models.User { Id = query.Id, UserName = "Frog", Balance = 100, Email = "frog@frog.com" };
            return user;
        }
    }
}