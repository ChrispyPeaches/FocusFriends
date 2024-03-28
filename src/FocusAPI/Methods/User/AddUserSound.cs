using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserSound
{
    public class Handler : IRequestHandler<AddUserSoundCommand, Unit>
    {
        FocusContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserSoundCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                FocusAPI.Models.Sound sound = await _context.Sounds.FirstOrDefaultAsync(s => s.Id == command.SoundId);

                user.Sounds.Add(new UserSound
                {
                    Sound = sound,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.Log(LogLevel.Error, "Error adding UserSound to database. Exception: " + ex.Message);
            }

            return Unit.Value;
        }
    }
}