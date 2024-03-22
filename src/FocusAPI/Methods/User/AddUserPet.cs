using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using FocusCore.Models;

namespace FocusApi.Methods.User;
public class AddUserPet
{
    public class Handler : IRequestHandler<AddUserPetCommand, Unit>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddUserPetCommand command, CancellationToken cancellationToken)
        {
            FocusAPI.Models.User user = _context.Users.First(u => u.Id == command.UserId);
            FocusAPI.Models.Pet pet = _context.Pets.First(p => p.Id == command.PetId);

            _context.UserPets.Add(new UserPet
            { 
                User = user,
                Pet = pet,
                DateAcquired = DateTime.UtcNow,
            });

            user.Balance = command.UpdatedBalance;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}