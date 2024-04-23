using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApp.Methods.User;
public class EditUserSelectedDecor
{
    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }

    internal class Handler : IRequestHandler<EditUserSelectedDecorCommand, MediatrResult>
    {
        FocusAppContext _localContext;
        ILogger<Handler> _logger;
        IAuthenticationService _authenticationService;
        IAPIClient _client;
        public Handler(FocusAppContext localContext, ILogger<Handler> logger, IAuthenticationService authenticationService, IAPIClient client)
        {
            _localContext = localContext;
            _logger = logger;
            _authenticationService = authenticationService;
            _client = client;
        }

        public async Task<MediatrResult> Handle(EditUserSelectedDecorCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Update the user on the server
                await _client.EditUserSelectedDecor(command, cancellationToken);

                // Update the local database to reflect the changes made to the user
                Shared.Models.User? user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

                // Fetch decor from local db if not null
                if (command.DecorId != null)
                {
                    Decor? decor = await _localContext.Decor.Where(d => command.DecorId == d.Id).FirstOrDefaultAsync(cancellationToken);

                    // Update the user's selected decor
                    user.SelectedDecor = decor;
                    user.SelectedDecorId = decor.Id;

                    // Update the authentication service to reflect the new selected decor
                    _authenticationService.SelectedDecor = decor;
                }
                else
                {
                    // Update the user's selected decor to null
                    user.SelectedDecor = null;
                    user.SelectedDecorId = null;

                    // Update the authentication service to reflect the null decor
                    _authenticationService.SelectedDecor = null;
                }

                await _localContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user selected decor.");
                return new MediatrResult
                {
                    Message = "Error changing user selected decor. Message: " + ex.Message,
                    Success = false
                };
            }

            return new MediatrResult
            {
                Message = "User selected decor changed successfully",
                Success = true
            };
        }
    }
}