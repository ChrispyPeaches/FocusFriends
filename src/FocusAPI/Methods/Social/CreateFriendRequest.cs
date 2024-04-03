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
public class CreateFriendRequest
{
    public class Handler : IRequestHandler<CreateFriendRequestCommand, Unit>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateFriendRequestCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                FocusAPI.Models.User friend = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.FriendId);

                user.Invitees?.Add(new Friendship
                {
                    User = user,
                    Friend = friend,
                    Status = 0
                });

                friend.Inviters?.Add(new Friendship
                {
                    User = friend,
                    Friend = user,
                    Status = 0
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error adding Friendship to database. Exception: " + ex.Message);
            }

            return Unit.Value;
        }
    }
}