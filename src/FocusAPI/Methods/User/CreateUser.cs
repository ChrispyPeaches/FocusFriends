using FocusCore.Commands.User;
using FocusCore.Models.User;
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
            UserModel user = new UserModel
            {
                Name = command.Name
            };

            // TODO: Insert new user into database

            return Unit.Value;
        }
    }
}