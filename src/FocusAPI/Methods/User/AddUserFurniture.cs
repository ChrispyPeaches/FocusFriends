using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;

namespace FocusApi.Methods.User;
public class AddUserFurniture
{
    public class Handler : IRequestHandler<AddUserFurnitureCommand, Unit>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddUserFurnitureCommand command, CancellationToken cancellationToken)
        {
            FocusAPI.Models.User user = _context.Users.First(u => u.Id == command.UserId);
            FocusAPI.Models.Furniture furniture = _context.Furniture.First(f => f.Id == command.FurnitureId);

            user.Furniture.Add(new UserFurniture
            {
                Furniture = furniture,
                DateAcquired = DateTime.UtcNow
            });

            user.Balance = command.UpdatedBalance;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}