using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Models;
using FocusCore.Queries.Shop;
using FocusCore.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;

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

        [HttpPost]
        [Route("FriendRequest")]
        public async Task CreateFriendRequest(CreateFriendRequestCommand command)
        {
            await _mediator.Send(command);
        }
    }
}