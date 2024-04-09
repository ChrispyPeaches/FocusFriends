using System.Net;
using FocusAPI.Methods.User;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusCore.Responses;
using FocusCore.Responses.User;

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

        /// <summary>
        /// Feature: <see cref="Methods.User.GetUser"/>
        /// </summary>
        [HttpGet]
        [Route("GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetUserResponse>> GetUserByAuth0Id(
            [FromQuery] GetUserQuery query,
            CancellationToken cancellationToken)
        {
            MediatrResultWrapper<GetUserResponse> result = new();

            try
            {
                result = await _mediator.Send(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500]: Error getting user");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            switch (result.HttpStatusCode)
            {
                case null:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                case HttpStatusCode.OK:
                    return Ok(result.Data);
                default:
                    _logger.LogError($"[{(int)result.HttpStatusCode}] {result.Message}");
                    return StatusCode((int)result.HttpStatusCode);
            }
        }

        /// <summary>
        /// Feature: <see cref="Methods.User.CreateUser"/>
        /// </summary>
        [HttpPost]
        [Route("CreateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateUserResponse>> CreateUser(
            CreateUserCommand command,
            CancellationToken cancellationToken)
        {
            MediatrResultWrapper<CreateUserResponse> result = new();

            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500]: Error creating user");
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

        /// <summary>
        /// Feature: <see cref="Methods.User.AddSessionToUser"/>
        /// </summary>
        [HttpPost]
        [Route("AddSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddSessionToUser(
            [FromBody] CreateSessionCommand query,
            CancellationToken cancellationToken = default)
        {
            MediatrResult result = new();

            try
            {
                result = await _mediator.Send(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500]: Error getting user");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            switch (result.HttpStatusCode)
            {
                case null:
                    _logger.LogError($"[500] {result.Message}");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                case HttpStatusCode.OK:
                    return Ok();
                default:
                    _logger.LogError($"[{(int)result.HttpStatusCode}] {result.Message}");
                    return StatusCode((int)result.HttpStatusCode);
            }
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