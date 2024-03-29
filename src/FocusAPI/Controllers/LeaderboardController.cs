using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusCore.Queries.Leaderboard;
using FocusCore.Models;

namespace FocusAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IMediator _mediator;

        public LeaderboardController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<LeaderboardDto>> GetDailyLeaderboard([FromQuery] GetDailyLeaderboardQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }
    }
}