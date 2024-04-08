using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApi.Methods.Social;
public class CancelFriendRequest
{
    public class Handler : IRequestHandler<CancelFriendRequestCommand, Unit>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(CancelFriendRequestCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var friendship = await _context.Friends
                    .Include(f => f.User)
                    .Include(f => f.Friend)
                    .FirstOrDefaultAsync(f => f.UserId == command.UserId && f.FriendId == command.FriendId);

                friendship.User.Inviters?.Remove(friendship);

                friendship.Friend.Invitees?.Remove(friendship);

                _context.Friends.Remove(friendship);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error removing friendship from database. Exception: " + ex.Message);
            }

            return Unit.Value;
        }
    }
}