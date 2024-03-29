using MediatR;
using FocusCore.Models;

namespace FocusCore.Queries.Leaderboard;
public class GetDailyLeaderboardQuery : IRequest<List<LeaderboardDto>> 
{
    public Guid UserId { get; set; }
}