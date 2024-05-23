using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApi.Methods.Social;
public class DeleteFriend
{
    public class Handler : IRequestHandler<DeleteFriendCommand, Unit>
    {
        FocusAPIContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteFriendCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch friendship
                var friendship = await _context.Friends
                    .Include(f => f.User)
                    .Include(f => f.Friend)
                    .FirstOrDefaultAsync(f => f.UserId == command.UserId && f.FriendId == command.FriendId);

                // Remove friend entry
                friendship.User.Inviters?.Remove(friendship);

                friendship.Friend.Invitees?.Remove(friendship);

                _context.Friends.Remove(friendship);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing friendship from database");
            }

            return Unit.Value;
        }
    }
}