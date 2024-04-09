using MediatR;
using FocusCore.Responses.Leaderboard;

namespace FocusCore.Queries.Leaderboard;
public class GetDailyLeaderboardQuery : IRequest<LeaderboardResponse> 
{
    public Guid UserId { get; set; }
}