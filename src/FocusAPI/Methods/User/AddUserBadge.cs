using System.Net;
using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using FocusCore.Responses;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserBadge
{
    public class Handler : IRequestHandler<AddUserBadgeCommand, Unit>
    {
        FocusAPIContext _apiContext;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext apiContext, ILogger<Handler> logger)
        {
            _apiContext = apiContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            AddUserBadgeCommand command,
            CancellationToken cancellationToken = default)
        {
            Models.User user = await _apiContext.Users
                .FirstAsync(u => u.Id == command.UserId, cancellationToken);

            Badge badge = await _apiContext.Badges
                .FirstAsync(p => p.Id == command.BadgeId, cancellationToken);

            user.Badges?.Add(new UserBadge
            {
                Badge = badge,
                DateAcquired = DateTime.UtcNow
            });

            await _apiContext.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}