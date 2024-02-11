using FocusAPI.Commands.User;
using FocusAPI.Models;
using MediatR;

namespace FocusApi.Methods.User;
public class CreateUser
{
    public class Handler : IRequestHandler<CreateUserCommand, Unit>
    {
        // TODO: Add db client reference
        public Handler() { }

        public async Task<Unit> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            FocusAPI.Models.User user = new FocusAPI.Models.User
            {
                Id = Guid.NewGuid()
            };

            // TODO: Insert new user into database

            return Unit.Value;
        }
    }
}