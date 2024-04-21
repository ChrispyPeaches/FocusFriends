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
public class EditUserSelectedPet
{
    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }

    internal class Handler : IRequestHandler<EditUserSelectedPetCommand, MediatrResult>
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

        public async Task<MediatrResult> Handle(EditUserSelectedPetCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Update the user on the server
                await _client.EditUserSelectedPet(command, cancellationToken);

                // Update the local database to reflect the changes made to the user
                Shared.Models.User? user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

                // Fetch pet from local db
                Pet? pet = await _localContext.Pets.Where(p => command.PetId == p.Id).FirstOrDefaultAsync(cancellationToken);

                // Update the user's selected pet
                user.SelectedPet = pet;
                user.SelectedPetId = pet.Id;

                // Update the authentication service to reflect the new selected pet
                _authenticationService.SelectedPet = pet;

                await _localContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user selected pet.");
                return new MediatrResult
                {
                    Message = "Error changing user selected pet. Message: " + ex.Message,
                    Success = false
                };
            }

            return new MediatrResult
            {
                Message = "User selected pet changed successfully",
                Success = true
            };
        }
    }
}