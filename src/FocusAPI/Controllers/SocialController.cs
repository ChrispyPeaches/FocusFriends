using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Commands.Social;
using FocusCore.Queries.Social;
using FocusCore.Responses;
using System.Net;
using System.Threading;
using FocusCore.Responses.Social;

namespace FocusAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SocialController : ControllerBase
    {
        private readonly ILogger<SocialController> _logger;
        private IMediator _mediator;

        public SocialController(ILogger<SocialController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("AllFriends")]
        public async Task<ActionResult<List<FriendListModel>>> GetAllFriends([FromQuery] GetAllFriendsQuery query, CancellationToken cancellationToken)
        {
            List<FriendListModel> result = new List<FriendListModel>();

            try
            {
                result = await _mediator.Send(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching friends");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(result);
        }


        [HttpGet]
        [Route("AllFriendRequests")]
        public async Task<ActionResult<List<FriendRequest>>> GetAllFriendRequests([FromQuery] GetAllFriendRequestsQuery query, CancellationToken cancellationToken)
        {
            List<FriendRequest> result = new List<FriendRequest>();

            try
            {
                result = await _mediator.Send(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching friend requests");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("FriendRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateFriendRequestResponse>> CreateFriendRequest(CreateFriendRequestCommand command, CancellationToken cancellationToken)
        {
            MediatrResultWrapper<CreateFriendRequestResponse> result = new();

            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500]: Error creating friend request");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            switch (result.HttpStatusCode)
            {
                case null:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                case HttpStatusCode.OK:
                    return Ok(result.Data);
                default:
                    _logger.LogError($"[{(int)result.HttpStatusCode}]: {result.Message}");
                    return StatusCode((int)result.HttpStatusCode);
            }
        }

        [HttpPut]
        [Route("FriendRequest")]
        public async Task<ActionResult> AcceptFriendRequest(AcceptFriendRequestCommand command)
        {
            try
            {
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting friend request");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("FriendRequest")]
        public async Task<ActionResult> CancelFriendRequest([FromBody] CancelFriendRequestCommand command)
        {
            try
            {
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling friend request");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok();
        }

    }
}