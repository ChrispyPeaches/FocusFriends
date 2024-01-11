using FocusCore.Models.User;
using FocusCore.Queries.User;
using MediatR;

namespace FocusApi.Methods.User;
public class GetUser
{
    public class Handler : IRequestHandler<GetUserQuery, UserModel>
    {
        // TODO: Add db client references
        public Handler() { }

        public async Task<UserModel> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            return new UserModel { Id = query.Id, Name = "Gunter" };
        }
    }
}