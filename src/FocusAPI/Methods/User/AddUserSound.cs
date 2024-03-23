using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;

namespace FocusApi.Methods.User;
public class AddUserSound
{
    public class Handler : IRequestHandler<AddUserSoundCommand, Unit>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddUserSoundCommand command, CancellationToken cancellationToken)
        {
            FocusAPI.Models.User user = _context.Users.First(u => u.Id == command.UserId);
            FocusAPI.Models.Sound sound = _context.Sounds.First(s => s.Id == command.SoundId);

            user.Sounds.Add(new UserSound
            {
                Sound = sound,
                DateAcquired = DateTime.UtcNow
            });

            user.Balance = command.UpdatedBalance;

            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}