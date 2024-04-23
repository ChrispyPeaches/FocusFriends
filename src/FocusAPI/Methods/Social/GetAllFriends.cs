using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApi.Methods.Social;
public class GetAllFriends
{
    public class Handler : IRequestHandler<GetAllFriendsQuery, List<FriendListModel>>
    {
        FocusAPIContext _context;
        public Handler(FocusAPIContext context)
        {
            _context = context;
        }

        public async Task<List<FriendListModel>> Handle(GetAllFriendsQuery query, CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .Include(user => user.Inviters)
                .ThenInclude(i => i.Friend)
                .Include(user => user.Invitees)
                .ThenInclude(i => i.User)
                .Where(user => user.Id == query.UserId).FirstOrDefaultAsync(cancellationToken);

            // Friends where user is the inviter
            List<FriendListModel> inviters = user.Inviters.Where(f => f.Status == 1).Select(f => new FriendListModel
            {
                FriendUserName = f.Friend.UserName,
                FriendEmail = f.Friend.Email,
                FriendProfilePicture = f.Friend.ProfilePicture,
                FriendAuth0Id = f.Friend.Auth0Id
            })
            .ToList();

            // Friends where user is the invitee
            List<FriendListModel> invitees = user.Invitees.Where(f => f.Status == 1).Select(f => new FriendListModel
            {
                FriendUserName = f.User.UserName,
                FriendEmail = f.User.Email,
                FriendProfilePicture = f.User.ProfilePicture,
                FriendAuth0Id = f.User.Auth0Id
            })
            .ToList();

            List<FriendListModel> friendsList = inviters.Concat(invitees).OrderBy(f => f.FriendUserName).ToList();

            return friendsList;
        }
    }
}