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
            return Unit.Value;
        }
    }
}