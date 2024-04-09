using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserFurniture
{
    public class Handler : IRequestHandler<AddUserFurnitureCommand, Unit>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserFurnitureCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                FocusAPI.Models.Furniture furniture = await _context.Furniture.FirstOrDefaultAsync(f => f.Id == command.FurnitureId);

                user.Furniture.Add(new UserFurniture
                {
                    Furniture = furniture,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error adding UserFurniture to database. Exception: " + ex.Message);
            }

            return Unit.Value;
        }
    }
}