using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.Social;
using FocusCore.Commands.User;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;

namespace FocusApi.Methods.Social;
public class CreateFriendship
{
    public class Handler// : IRequestHandler<CreateFriendshipCommand, BaseFriendship>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async void Handle(CreateFriendshipCommand command, CancellationToken cancellationToken)
        {
        }
    }
}