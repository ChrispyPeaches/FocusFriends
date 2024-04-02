using MediatR;
using FocusCore.Models;

namespace FocusCore.Queries.Leaderboard;
public class GetWeeklyLeaderboardQuery : IRequest<List<LeaderboardDto>>
{
    public Guid UserId { get; set; }
}