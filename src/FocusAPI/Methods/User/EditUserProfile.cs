using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FocusAPI.Data;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FocusAPI.Methods.User;
public class EditUserProfile
{
    public class Handler : IRequestHandler<EditUserProfileCommand, MediatrResult>
    {
        FocusAPIContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MediatrResult> Handle(EditUserProfileCommand command, CancellationToken cancellationToken) 
        {
            Models.User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == command.UserId);

            if (user == null)
                return new MediatrResult
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "User not found"
                };

            try
            {
                // Update the user's fields if the command field is not null, otherwise, leave the same
                user.UserName = command.UserName == null ? user.UserName : command.UserName;
                user.Pronouns = command.Pronouns == null ? user.Pronouns : command.Pronouns;
                user.ProfilePicture = command.ProfilePicture == null ? user.ProfilePicture : command.ProfilePicture;

                await _context.SaveChangesAsync();

                return new MediatrResult { HttpStatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error saving changes to user profile in database.");
                
                return new MediatrResult
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "Error saving changes to user profile in database."
                };
            }
        }
    }
}

