using System.Net;
using FocusCore.Commands.User;
using MediatR;
using FocusAPI.Data;
using FocusAPI.Models;
using Microsoft.EntityFrameworkCore;
using FocusCore.Responses;

namespace FocusAPI.Methods.User;
public class AddSessionToUser
{
    public class Handler : IRequestHandler<CreateSessionCommand, MediatrResult>
    {
        private readonly FocusContext _context;

        public Handler(
            FocusContext context)
        {
            _context = context;
        }

        public async Task<MediatrResult> Handle(
            CreateSessionCommand command,
            CancellationToken cancellationToken = default)
        {
            FocusAPI.Models.User? user = await GetUser(command.Auth0Id, cancellationToken);
               
            // If the user exists, add the session to their sessions
            if (user != null)
            {
                await AddSessionToUser(command, user, cancellationToken);

                return new MediatrResult { HttpStatusCode = HttpStatusCode.OK };
            }
            else
            {
                return new MediatrResult
                { 
                    HttpStatusCode = HttpStatusCode.NotFound, 
                    Message = $"User not found with Auth0Id: {command.Auth0Id}",
                };
            }
        }

        private async Task<FocusAPI.Models.User?> GetUser(
            string? auth0Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.UserSessions)
                    .Where(u => u.Auth0Id == auth0Id)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user", ex);
            }
        }

        private async Task AddSessionToUser(
            CreateSessionCommand command,
            Models.User? user,
            CancellationToken cancellationToken)
        {
            try
            {
                UserSession session = new UserSession()
                {
                    Id = command.SessionId,
                    SessionStartTime = command.SessionStartTime,
                    SessionEndTime = command.SessionEndTime,
                    CurrencyEarned = command.CurrencyEarned,
                    User = user,
                    UserId = user.Id
                };

                await _context.UserSessionHistory.AddAsync(session, cancellationToken);

                user?.UserSessions?.Add(session);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding session to user.", ex);
            }
        }
    }
}