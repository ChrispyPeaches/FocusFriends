using FocusAPI.Data;
using FocusAPI.Methods.User;
using FocusAPI.Models;
using FocusCore.Commands.Social;
using FocusCore.Models;
using FocusCore.Responses;
using FocusCore.Responses.Social;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FocusApi.Methods.Social;
public class CreateFriendRequest
{
    public class Handler : IRequestHandler<CreateFriendRequestCommand, MediatrResultWrapper<CreateFriendRequestResponse>>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MediatrResultWrapper<CreateFriendRequestResponse>> Handle(CreateFriendRequestCommand command, CancellationToken cancellationToken = default)
        {
            // First we check if friendship already exists
            Friendship? existingFriendship = await GetFriendship(command, cancellationToken);

            if (existingFriendship != null)
            {
                return new MediatrResultWrapper<CreateFriendRequestResponse>
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = $"Friendship already exists: {existingFriendship.User.Email} - {existingFriendship.Friend.Email}"
                };
            }

            FocusAPI.Models.User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.UserEmail);
            FocusAPI.Models.User friend = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.FriendEmail);

            // Check if friend exists (Email is invalid)
            if (friend == null)
            {
                return new MediatrResultWrapper<CreateFriendRequestResponse>
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = $"Friend email is invalid: {command.FriendEmail}"
                };
            }
            else
            {
                // Create new friend request entry (friendship relationship with status 0)
                Friendship friendship = new Friendship
                {
                    User = user,
                    Friend = friend,
                    Status = 0
                };

                user.Inviters?.Add(friendship);

                friend.Invitees?.Add(friendship);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating friend request");
                    return new MediatrResultWrapper<CreateFriendRequestResponse>
                    {
                        HttpStatusCode = HttpStatusCode.InternalServerError,
                        Message = $"Error creating friend request"
                    };
                }

                return new MediatrResultWrapper<CreateFriendRequestResponse>()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Data = new CreateFriendRequestResponse
                    {
                        PendingFriendRequest = new BaseFriendship
                        {
                            User = user,
                            Friend = friend,
                            Status = 0
                        }
                    }
                };
            }
        }

        private async Task<Friendship?> GetFriendship(
            CreateFriendRequestCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Friends
                    .Where(f => (f.User.Email == command.UserEmail && f.Friend.Email == command.FriendEmail)
                    || (f.User.Email == command.FriendEmail && f.Friend.Email == command.UserEmail) )
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting friendship");
                return null;
            }
        }
    }
}