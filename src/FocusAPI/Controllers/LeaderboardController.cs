using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusCore.Queries.Leaderboard;
using FocusCore.Models;
using FocusCore.Responses.Leaderboard;

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
        [Route("Daily")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LeaderboardResponse>> GetDailyLeaderboard([FromQuery] GetDailyLeaderboardQuery query, CancellationToken cancellationToken)
        {
            try
            {
                LeaderboardResponse result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex) 
            {
                _logger.Log(LogLevel.Debug, "Error retreiving daily leaderboards. Message: " + ex.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Weekly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LeaderboardResponse>> GetWeeklyLeaderboard([FromQuery] GetWeeklyLeaderboardQuery query, CancellationToken cancellationToken)
        {
            try
            {
                LeaderboardResponse result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, "Error retreiving weekly leaderboards. Message: " + ex.Message);
                return StatusCode(500);
            }
        }
    }
}