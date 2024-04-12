using FocusAPI.Helpers;
using FocusAPI.Methods.Sync;
using FocusAPI.Models;
using FocusCore.Models;
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
                //SyncMindfulnessTipsResponse result = await _mediator.Send(query, cancellationToken);
                var result1 = await _mediator.Send(new SyncItems.Query<MindfulnessTip>()
                {
                    ItemIds = query.MindfulnessTipIds
                },
                cancellationToken);

                return Ok(new SyncMindfulnessTipsResponse()
                {
                    MissingTips = result1.MissingItems
                        .Select(ProjectionHelper.ProjectToBaseMindfulnessTip)
                        .ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mindfulness tip ids");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("InitialData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncInitialDataResponse>> InitialData(
            [FromBody] SyncInitialDataQuery query,
            CancellationToken cancellationToken = default)
        {
            try
            {
                SyncInitialDataResponse result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting initial data");
                return StatusCode(500);
            }
        }
    }
}