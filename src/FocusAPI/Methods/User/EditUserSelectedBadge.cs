using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FocusAPI.Methods.User;
public class EditUserSelectedBadge
{
    public class Handler : IRequestHandler<EditUserSelectedBadgeCommand, MediatrResult>
    {
        FocusAPIContext _context;
        ILogger<Handler> _logger;

        public Handler(FocusAPIContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MediatrResult> Handle(EditUserSelectedBadgeCommand command, CancellationToken cancellationToken)
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
                Badge badge = await _context.Badges.Where(b => command.BadgeId == b.Id).FirstOrDefaultAsync(cancellationToken);

                // Update the user's selected badge
                user.SelectedBadge = badge;
                user.SelectedBadgeId = badge.Id;

                await _context.SaveChangesAsync();

                return new MediatrResult { HttpStatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to user selected badge in database");
            }
            return new MediatrResult
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                Message = "Error saving changes to user selected badge in database."
            };
        }
        
    }
}
