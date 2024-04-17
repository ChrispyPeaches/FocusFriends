using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using FocusCore.Queries.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApi.Methods.Social;
public class GetAllFriendRequests
{
    public class Handler : IRequestHandler<GetAllFriendRequestsQuery, List<FriendRequest>>
    {
        FocusAPIContext _context;
        public Handler(FocusAPIContext context)
        {
            _context = context;
        }

        public async Task<List<FriendRequest>> Handle(GetAllFriendRequestsQuery query, CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .Include(user => user.Inviters)
                .ThenInclude(i => i.Friend)
                .Include(user => user.Invitees)
                .ThenInclude(i => i.User)
                .Where(user => user.Id == query.UserId).FirstOrDefaultAsync(cancellationToken);

            // Friend requests where user is the inviter
            List<FriendRequest> inviters = user.Inviters.Where(f => f.Status == 0).Select(f => new FriendRequest
            {
                FriendId = f.FriendId,
                FriendUserName = f.Friend.UserName,
                FriendEmail = f.Friend.Email,
                UserInitiated = true
            })
            .ToList();

            // Friend requests where user is the invitee
            List<FriendRequest> invitees = user.Invitees.Where(f => f.Status == 0).Select(f => new FriendRequest
            {
                FriendId = f.UserId,
                FriendUserName = f.User.UserName,
                FriendEmail = f.User.Email,
                UserInitiated = false
            })
            .ToList();

            List<FriendRequest> allRequests = inviters.Concat(invitees).OrderBy(fr => fr.UserInitiated).ToList();

            return allRequests;
        }
    }
}