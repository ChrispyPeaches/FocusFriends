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
            return Unit.Value;
        }
    }
}