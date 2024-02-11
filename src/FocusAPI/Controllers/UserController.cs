using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Commands.User;
using FocusAPI.Queries.User;
using FocusCore.Models.User;
using FocusAPI.Models;

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
        public async Task<User> GetUser([FromQuery] GetUserQuery query)
        {
            return await _mediator.Send(new GetUserQuery { Id = query.Id });
        }

        [HttpPost]
        public async Task CreateUser(CreateUserCommand command)
        {
            await _mediator.Send(command);
        }
    }
}