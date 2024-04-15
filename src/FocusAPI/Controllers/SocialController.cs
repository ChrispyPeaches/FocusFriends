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
        private readonly ILogger<ShopController> _logger;
        private IMediator _mediator;

        public SocialController(ILogger<ShopController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("AllFriends")]
        public async Task<List<FriendListModel>> GetAllFriends([FromQuery] GetAllFriendsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }


        [HttpGet]
        [Route("AllFriendRequests")]
        public async Task<List<FriendRequest>> GetAllFriendRequests([FromQuery] GetAllFriendRequestsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
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
        public async Task AcceptFriendRequest(AcceptFriendRequestCommand command)
        {
            await _mediator.Send(command);
        }

        [HttpDelete]
        [Route("FriendRequest")]
        public async Task CancelFriendRequest([FromBody] CancelFriendRequestCommand command)
        {
            await _mediator.Send(command);
        }

    }
}