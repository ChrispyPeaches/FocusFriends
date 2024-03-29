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
            // Grab friend Ids from database
            List<Guid> friendIds = await _context.Friends
                .Where(f => f.UserId == query.UserId)
                .Select(f => f.FriendId)
                .ToListAsync(cancellationToken);

            // Grab friends and requesting user's information
            List<FocusAPI.Models.User> users = await _context.Users
                .Where(u => friendIds.Contains(u.Id) || u.Id == query.UserId)
                .ToListAsync(cancellationToken);

            List<Guid> userIds = users.Select(u => u.Id).ToList();
            // Query session history table for friend's and requesting user's session history over a day's time
            List<UserSession> sessions = await _context.UserSessionHistory
                .Where(s =>
                       userIds.Contains(s.UserId)
                    && s.SessionEndTime <= DateTime.UtcNow
                    && s.SessionEndTime > DateTime.UtcNow.AddDays(-1))
                .ToListAsync(cancellationToken);

            // Aggregate daily stats for users
            int currencyEarned;
            List<LeaderboardDto> leaderboard = new List<LeaderboardDto>();
            foreach (Guid userId in userIds)
            {
                List<UserSession> userSessions = sessions.Where(s => s.UserId == userId).ToList();
                currencyEarned = userSessions.Sum(s => s.CurrencyEarned);

                FocusAPI.Models.User user = users.First(u => u.Id == userId);
                leaderboard.Add(new LeaderboardDto
                {
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture,
                    CurrencyEarned = currencyEarned,
                });
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
    }
}