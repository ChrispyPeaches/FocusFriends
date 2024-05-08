using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusCore.Commands.User;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Methods.User;
internal class AddSessionToUser
{
    public class Query : IRequest
    {
        public DateTimeOffset SessionStartTime { get; set; }
        public DateTimeOffset SessionEndTime { get; set; }
        public int CurrencyEarned { get; set; }
    }

    public class Handler : IRequestHandler<Query>
    {
        private readonly FocusAppContext _context;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<Handler> _logger;
        private readonly IAPIClient _client;
        private readonly IBadgeService _badgeService;

        public Handler(
            FocusAppContext context,
            IAuthenticationService authService,
            ILogger<Handler> logger,
            IAPIClient client,
            IBadgeService badgeService)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
            _client = client;
            _badgeService = badgeService;
        }

        public async Task Handle(
            Query query,
            CancellationToken cancellationToken = default)
        {
            // If a user isn't logged in, don't track the session
            if (_authService?.Auth0Id is null) return;

            Shared.Models.User? user = await GetUser(cancellationToken);

            UserSession session = CreateSession(query, user);

            _authService.Balance += session.CurrencyEarned;

            // If a user isn't found, don't track the session
            if (user is null) return;

            await AddSessionToServerUser(
                query,
                session.Id,
                _authService.Auth0Id,
                session.CurrencyEarned,
                cancellationToken);

            await AddSessionToMobileDatabaseUser(session, user, cancellationToken);
        }

        private async Task<Shared.Models.User?> GetUser(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.UserSessions)
                    .Where(u => u.Auth0Id == _authService.Auth0Id)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user", ex);
            }
        }

        private static UserSession CreateSession(
            Query query,
            Shared.Models.User user) =>
            new()
            {
                Id = Guid.NewGuid(),
                SessionStartTime = query.SessionStartTime.UtcDateTime,
                SessionEndTime = query.SessionEndTime.UtcDateTime,
                CurrencyEarned = CalculateCurrencyEarned(query),
                User = user,
                UserId = user.Id
            };

        private async Task AddSessionToServerUser(
            Query query,
            Guid sessionId,
            string auth0Id,
            int currencyEarned,
            CancellationToken cancellationToken)
        {
            try
            {
                await _client.CreateSession(new CreateSessionCommand()
                {
                    Auth0Id = auth0Id,
                    SessionId = sessionId,
                    SessionStartTime = query.SessionStartTime,
                    SessionEndTime = query.SessionEndTime,
                    CurrencyEarned = currencyEarned
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error adding session to user on server.");
            }
        }

        private async Task AddSessionToMobileDatabaseUser(
            UserSession session,
            Shared.Models.User user,
            CancellationToken cancellationToken)
        {
            try
            {
                await _context.AddAsync(session, cancellationToken);
                user.UserSessions?.Add(session);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error adding session to user in mobile database.");
            }
        }

        private static int CalculateCurrencyEarned(Query query)
        {
            int currencyEarned = (int)Math.Ceiling((query.SessionEndTime - query.SessionStartTime).TotalMinutes);

            // Ensure the value is not negative
            currencyEarned = currencyEarned <= 0 ? 0 : currencyEarned;

            return currencyEarned;
        }
    }
}