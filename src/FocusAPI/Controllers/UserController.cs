using System.Net;
using FocusAPI.Methods.User;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusCore.Responses;
using FocusCore.Responses.User;
using System.Threading;

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
        [Route("Edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserProfile(
            EditUserProfileCommand command,
            CancellationToken cancellationToken = default)
        {

            MediatrResult result = new();
            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user profile details.");
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
        [Route("Decor")]
        public async Task AddUserDecor(AddUserDecorCommand command)
        {
            await _mediator.Send(command);
        }

        
        [HttpPost]
        [Route("Island")]
        public async Task AddUserIsland(AddUserIslandCommand command)
        {
            await _mediator.Send(command);
        }

        [HttpPost]
        [Route("Badge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddUserBadge(
            AddUserBadgeCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.Send(command, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user profile details.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("EditUserSelectedPet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserSelectedPet(
            EditUserSelectedPetCommand command,
            CancellationToken cancellationToken = default)
        {

            MediatrResult result = new();
            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user selected pet.");
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

        [HttpPut]
        [Route("EditUserSelectedDecor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserSelectedDecor(
            EditUserSelectedDecorCommand command,
            CancellationToken cancellationToken = default)
        {

            MediatrResult result = new();
            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user selected decor.");
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

        [HttpPut]
        [Route("EditUserSelectedIsland")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserSelectedIslands(
            EditUserSelectedIslandCommand command,
            CancellationToken cancellationToken = default)
        {

            MediatrResult result = new();
            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user selected island.");
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

        [HttpPut]
        [Route("EditUserSelectedBadge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserSelectedBadge(
            EditUserSelectedBadgeCommand command,
            CancellationToken cancellationToken = default)
        {
            MediatrResult result = new();
            try
            {
                result = await _mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[500] Error editing user selected badge.");
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
    }
}