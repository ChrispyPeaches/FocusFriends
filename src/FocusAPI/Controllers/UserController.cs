using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusAPI.Models;
using FocusCore.Models;

namespace FocusAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IMediator _mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<BaseUser> GetUserByAuth0Id([FromQuery] GetUserQuery query)
        {
            return await _mediator.Send(new GetUserQuery
            {
                Auth0Id = query.Auth0Id,
                Email = query.Email,
                UserName = query.UserName
            });
        }

        [HttpPost]
        public async Task CreateUser(CreateUserCommand command)
        {
            await _mediator.Send(command);
        }

        [HttpPost]
        [Route("Pet")]
        public async Task AddUserPet(AddUserPetCommand command)
        {
            await _mediator.Send(command);
        }

        [HttpPost]
        [Route("Furniture")]
        public async Task AddUserFurniture(AddUserFurnitureCommand command)
        {
            await _mediator.Send(command);
        }

        [HttpPost]
        [Route("Sound")]
        public async Task AddUserSound(AddUserSoundCommand command)
        {
            await _mediator.Send(command);
        }
    }
}