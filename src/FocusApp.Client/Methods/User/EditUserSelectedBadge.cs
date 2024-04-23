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
public class EditUserSelectedBadges
{
    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }

    internal class Handler : IRequestHandler<EditUserSelectedBadgeCommand, MediatrResult>
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

        public async Task<MediatrResult> Handle(EditUserSelectedBadgeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Update the user on the server
                await _client.EditUserSelectedBadge(command, cancellationToken);

                // Update the local database to reflect the changes made to the user
                Shared.Models.User? user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

                // Fetch badge from local db
                Badge? badge = await _localContext.Badges.Where(b => command.BadgeId == b.Id).FirstOrDefaultAsync(cancellationToken);

                // Update the user's selected badge
                user.SelectedBadge = badge;
                user.SelectedBadgeId = badge.Id;

                // Update the authentication service to reflect the new selected badge
                _authenticationService.SelectedBadge = badge;

                await _localContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user selected badge.");
                return new MediatrResult
                {
                    Message = "Error changing user selected badge. Message: " + ex.Message,
                    Success = false
                };
            }

            return new MediatrResult
            {
                Message = "User selected badge changed successfully",
                Success = true
            };
        }
    }
}