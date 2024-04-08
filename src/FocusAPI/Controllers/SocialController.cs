using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Models;
using FocusCore.Queries.Shop;
using FocusCore.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Queries.Social;

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
        [Route("FriendRequest")]
        public async Task<List<FriendRequest>> GetAllFriendRequests([FromQuery] GetAllFriendRequestsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [HttpPost]
        [Route("FriendRequest")]
        public async Task CreateFriendRequest(CreateFriendRequestCommand command)
        {
            await _mediator.Send(command);
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