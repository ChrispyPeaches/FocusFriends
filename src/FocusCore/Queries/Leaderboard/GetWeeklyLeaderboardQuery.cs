using MediatR;
using FocusCore.Responses.Leaderboard;

namespace FocusCore.Queries.Leaderboard;
public class GetWeeklyLeaderboardQuery : IRequest<LeaderboardResponse>
{
    public Guid UserId { get; set; }
}