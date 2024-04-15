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
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
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
                user.Pronouns = command.Pronouns;
                user.UserName = command.UserName;

                await _context.SaveChangesAsync();

                return new MediatrResult { HttpStatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex) 
            {
                _logger.Log(LogLevel.Error, "Error saving changes to user profile in database. Message: " + ex.Message);
                
                return new MediatrResult
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "Error saving changes to user profile in database."
                };
            }
        }
    }
}

