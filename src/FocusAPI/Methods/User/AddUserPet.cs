using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserPet
{
    public class Handler : IRequestHandler<AddUserPetCommand, Unit>
    {
        FocusAPIContext _apiContext;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext apiContext, ILogger<Handler> logger)
        {
            _apiContext = apiContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserPetCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = await _apiContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                Pet pet = await _apiContext.Pets.FirstOrDefaultAsync(p => p.Id == command.PetId);

                user.Pets?.Add(new UserPet
                {
                    Pet = pet,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _apiContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.Log(LogLevel.Error, "Error adding UserPet to database. Exception: " + ex.Message);
            }
            
            return Unit.Value;
        }
    }
}