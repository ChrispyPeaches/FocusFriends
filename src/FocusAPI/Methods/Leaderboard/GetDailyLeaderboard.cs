using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Queries.Leaderboard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusCore.Models;

namespace FocusApi.Methods.Leaderboard;
public class GetDailyLeaderboard
{
    public class Handler : IRequestHandler<GetDailyLeaderboardQuery, List<LeaderboardDto>>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<List<LeaderboardDto>> Handle(GetDailyLeaderboardQuery query, CancellationToken cancellationToken)
        {
            User? user = await _context.Users
                .Include(u => u.Inviters)
                    .ThenInclude(f => f.Friend)
                .Include(u => u.Invitees)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

            List<Guid> userIds = user.Inviters.Select(i => i.FriendId).Concat(user.Invitees.Select(i => i.UserId)).ToList();
            
            // Add requesting user's Id to userIds
            userIds.Add(query.UserId);

            // Query session history table for friend's and requesting user's session history over a day's time
            List<UserSession> sessions = await _context.UserSessionHistory
                .Where(s =>
                       userIds.Contains(s.UserId)
                    && s.SessionEndTime <= DateTime.UtcNow
                    && s.SessionEndTime > DateTime.UtcNow.AddDays(-1))
                .ToListAsync(cancellationToken);

            // If there are no sessions from today, return empty list
            if (!sessions.Any())
                return new List<LeaderboardDto>();

            // Aggregate daily stats for users
            int currencyEarned;
            List<LeaderboardDto> leaderboard = new List<LeaderboardDto>();
            foreach (Guid userId in userIds)
            {
                List<UserSession> userSessions = sessions.Where(s => s.UserId == userId).ToList();
                currencyEarned = userSessions.Sum(s => s.CurrencyEarned);

                if (userId == user.Id)
                {
                    leaderboard.Add(new LeaderboardDto
                    {
                        UserName = user?.UserName,
                        ProfilePicture = user?.ProfilePicture,
                        CurrencyEarned = currencyEarned,
                    });
                }
                else
                {
                    User userInfo = GetUserInfoFromFriends(user, userId);
                    leaderboard.Add(new LeaderboardDto
                    {
                        UserName = userInfo?.UserName,
                        ProfilePicture = userInfo?.ProfilePicture,
                        CurrencyEarned = currencyEarned,
                    });
                }
            }

            // Rank users by currency earned
            leaderboard = leaderboard.OrderByDescending(l => l.CurrencyEarned).ToList();

            int rank;
            for (int i = 0; i < leaderboard.Count; i++)
            {
                rank = i + 1;
                leaderboard[i].Rank = rank;
            }

            return leaderboard;
        }

        User GetUserInfoFromFriends(User user, Guid userId)
        {
            User userInfo;
            Friendship friendship = user.Invitees.FirstOrDefault(i => i.UserId == userId);
            if (friendship == null)
            {
                // Check inviters for friend object
                friendship = user.Inviters.FirstOrDefault(i => i.FriendId == userId);
                userInfo = friendship.Friend;
            }
            else
            {
                userInfo = friendship.User;
            }

            return userInfo;
        }
    }
}