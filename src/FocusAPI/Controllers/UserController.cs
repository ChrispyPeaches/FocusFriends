using Microsoft.AspNetCore.Mvc;
using FocusCore.Models;
using MediatR;
using FocusCore.Queries;
using FocusCore.Commands;

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
        public async Task<UserModel> GetUserData()
        {
            return await _mediator.Send(new GetUserDataQuery { Id = Guid.NewGuid() });
        }

        [HttpPost]
        public async Task CreateUser(CreateUserCommand command)
        {
            await _mediator.Send(command);
        }
    }
}