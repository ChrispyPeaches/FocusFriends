using FocusCore.Queries.User;
using FocusCore.Models;
using MediatR;
using FocusAPI.Data;
using FocusAPI.Models;

namespace FocusApi.Methods.User;
public class GetUser
{
    public class Handler : IRequestHandler<GetUserQuery, BaseUser>
    {
        FocusContext _context;
        public Handler(FocusContext context) 
        {
            _context = context;
        }

        public async Task<BaseUser> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            BaseUser user;

            List<BaseUser> users = _context.Users.OfType<BaseUser>().Where(u => u.Auth0Id == query.Auth0Id).ToList();

            // If the user does not yet exist in the database, create the user
            if (!users.Any())
            {
                user = new FocusAPI.Models.User
                {
                    Auth0Id = query.Auth0Id,
                    Id = Guid.NewGuid(),
                    UserName = query.UserName,
                    Email = query.Email,
                    Balance = 0
                };

                _context.Users.Add((FocusAPI.Models.User) user);

                await _context.SaveChangesAsync();
            }

            // If the user exists in the database, return the user
            else
            {
                user = users.First(u => u.Auth0Id == query.Auth0Id);
            }

            return user;
        }
    }
}