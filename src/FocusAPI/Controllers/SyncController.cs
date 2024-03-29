using FocusAPI.Methods.Sync;
using FocusAPI.Models;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace FocusAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly ILogger<SyncController> _logger;
        private IMediator _mediator;

        public SyncController(ILogger<SyncController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Feature: <see cref="SyncMindfulnessTips"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MindfulnessTips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncMindfulnessTipsResponse>> MindfulnessTips(
            [FromBody] SyncMindfulnessTipsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                SyncMindfulnessTipsResponse result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mindfulness tip ids");
                return StatusCode(500);
            }
        }
    }
}