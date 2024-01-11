using FocusCore.Models;
using FocusCore.Queries;
using MediatR;

namespace FocusApi.Methods.User;
public class GetUserData
{
    public class Handler : IRequestHandler<GetUserDataQuery, UserModel>
    {
        // TODO: Add db client references
        public Handler() { }

        public async Task<UserModel> Handle(GetUserDataQuery query, CancellationToken cancellationToken)
        {
            return new UserModel { Id = Guid.NewGuid(), Name = "Gunter" };
        }
    }
}