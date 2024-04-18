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

        /// <summary> Feature: <see cref="SyncItems"/> </summary>
        [HttpPost]
        [Route("MindfulnessTips")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncItemResponse<BaseMindfulnessTip>>> MindfulnessTips(
            [FromBody] SyncItemsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(
                    new SyncItems.Query<MindfulnessTip>
                    {
                        ItemIds = query.ItemIds
                    },
                    cancellationToken);

                return Ok(
                    new SyncItemResponse<BaseMindfulnessTip>
                    {
                        MissingItems = result.MissingItems
                            .Select(ProjectionHelper.ProjectToBaseMindfulnessTip)
                            .ToList()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing mindfulness tips");
                return StatusCode(500);
            }
        }

        /// <summary> Feature: <see cref="SyncItems"/> </summary>
        [HttpPost]
        [Route("Badges")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncItemResponse<BaseBadge>>> Badges(
            [FromBody] SyncItemsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(
                    new SyncItems.Query<Badge>
                    {
                        ItemIds = query.ItemIds
                    },
                    cancellationToken);

                return Ok(
                    new SyncItemResponse<BaseBadge>
                    {
                        MissingItems = result.MissingItems
                            .Select(ProjectionHelper.ProjectToBaseBadge)
                            .ToList()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing badges");
                return StatusCode(500);
            }
        }

        /// <summary> Feature: <see cref="SyncItems"/> </summary>
        [HttpPost]
        [Route("Pets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncItemResponse<BasePet>>> Pets(
            [FromBody] SyncItemsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(
                    new SyncItems.Query<Pet>
                    {
                        ItemIds = query.ItemIds
                    },
                    cancellationToken);

                return Ok(
                    new SyncItemResponse<BasePet>
                    {
                        MissingItems = result.MissingItems
                            .Select(ProjectionHelper.ProjectToBasePet)
                            .ToList()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing pets");
                return StatusCode(500);
            }
        }

        /// <summary> Feature: <see cref="SyncItems"/> </summary>
        [HttpPost]
        [Route("Decor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncItemResponse<BaseDecor>>> Decor(
            [FromBody] SyncItemsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(
                    new SyncItems.Query<Decor>
                    {
                        ItemIds = query.ItemIds
                    },
                    cancellationToken);

                return Ok(
                    new SyncItemResponse<BaseDecor>
                    {
                        MissingItems = result.MissingItems
                            .Select(ProjectionHelper.ProjectToBaseDecor)
                            .ToList()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing decor");
                return StatusCode(500);
            }
        }

        /// <summary> Feature: <see cref="SyncItems"/> </summary>
        [HttpPost]
        [Route("Islands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncItemResponse<BaseIsland>>> Islands(
            [FromBody] SyncItemsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(
                    new SyncItems.Query<Island>
                    {
                        ItemIds = query.ItemIds
                    },
                    cancellationToken);

                return Ok(
                    new SyncItemResponse<BaseIsland>
                    {
                        MissingItems = result.MissingItems
                            .Select(ProjectionHelper.ProjectToBaseIsland)
                            .ToList()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing islands");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Feature: <see cref="SyncBadges"/>
        /// </summary>
        [HttpPost]
        [Route("Badges")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SyncBadgesResponse>> MindfulnessTips(
            [FromBody] SyncBadgesQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                SyncBadgesResponse result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting mindfulness tip ids");
                return StatusCode(500);
            }
        }

        /// <summary> Feature: <see cref="SyncInitialData"/> </summary>
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