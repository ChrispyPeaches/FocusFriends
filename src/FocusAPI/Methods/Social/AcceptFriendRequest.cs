using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApi.Methods.Social;
public class AcceptFriendRequest
{
    public class Handler : IRequestHandler<AcceptFriendRequestCommand, Unit>
    {
        FocusAPIContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(AcceptFriendRequestCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var friendship = await _context.Friends
                    .FirstOrDefaultAsync(f => f.UserId == command.FriendId && f.FriendId == command.UserId);

                // Fulfill friend request
                friendship.Status = 1;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex , "Error editing friendship in database");
            }

            return Unit.Value;
        }
    }
}