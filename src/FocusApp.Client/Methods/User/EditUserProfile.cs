using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApp.Methods.User;
public class EditUserProfile
{
    public class Response
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }

    internal class Handler : IRequestHandler<EditUserProfileCommand, MediatrResult>
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

        public async Task<MediatrResult> Handle(EditUserProfileCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Update the user on the server
                await _client.EditUserProfile(command, default);

                // Update the local database to reflect the changes made to the user
                Shared.Models.User? user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);

                // Update the user's fields if the command field is not null, otherwise, leave the same
                user.UserName = command.UserName == null ? user.UserName : command.UserName;
                user.Pronouns = command.Pronouns == null ? user.Pronouns : command.Pronouns;
                user.ProfilePicture = command.ProfilePicture == null ? user.ProfilePicture : command.ProfilePicture;

                // Update the authentication service to reflect the new changes in the current session
                _authenticationService.CurrentUser.UserName = user.UserName;
                _authenticationService.CurrentUser.Pronouns = user.Pronouns;
                _authenticationService.CurrentUser.ProfilePicture = user.ProfilePicture;

                await _localContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, "Error editing user profile. Message: " + ex.Message);
                return new MediatrResult
                {
                    Message = "Error editing user profile. Message: " + ex.Message,
                    Success = false
                };
            }

            return new MediatrResult
            {
                Message = "User edited successfully",
                Success = true
            };
        }
    }
}