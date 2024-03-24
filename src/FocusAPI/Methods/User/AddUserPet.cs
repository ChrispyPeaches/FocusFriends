using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;

namespace FocusApi.Methods.User;
public class AddUserPet
{
    public class Handler : IRequestHandler<AddUserPetCommand, Unit>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserPetCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = _context.Users.First(u => u.Id == command.UserId);
                Pet pet = _context.Pets.First(p => p.Id == command.PetId);

                user.Pets?.Add(new UserPet
                {
                    Pet = pet,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.Log(LogLevel.Error, "Error adding UserPet to database. Exception: " + ex.Message);
            }
            
            return Unit.Value;
        }
    }
}