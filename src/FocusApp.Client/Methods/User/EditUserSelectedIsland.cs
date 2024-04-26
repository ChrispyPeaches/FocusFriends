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
public class EditUserSelectedIsland
{
    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }

    internal class Handler : IRequestHandler<EditUserSelectedIslandCommand, MediatrResult>
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

        public async Task<MediatrResult> Handle(EditUserSelectedIslandCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Update the user on the server
                await _client.EditUserSelectedIsland(command, cancellationToken);

                // Update the local database to reflect the changes made to the user
                Shared.Models.User? user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

                // Fetch island from local db if not null
                if (command.IslandId != null)
                {
                    Island? island = await _localContext.Islands.Where(i => command.IslandId == i.Id).FirstOrDefaultAsync(cancellationToken);

                    // Update the user's selected island
                    user.SelectedIsland = island;
                    user.SelectedIslandId = island.Id;

                    // Update the authentication service to reflect the new selected island
                    _authenticationService.SelectedIsland = island;
                }

                await _localContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user selected island.");
                return new MediatrResult
                {
                    Message = "Error changing user selected island. Message: " + ex.Message,
                    Success = false
                };
            }

            return new MediatrResult
            {
                Message = "User selected island changed successfully",
                Success = true
            };
        }
    }
}