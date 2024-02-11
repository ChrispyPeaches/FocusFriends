using FocusAPI.Queries.User;
using FocusAPI.Models;
using MediatR;

namespace FocusApi.Methods.User;
public class GetUser
{
    public class Handler : IRequestHandler<GetUserQuery, FocusAPI.Models.User>
    {
        // TODO: Add db client references
        public Handler() { }

        public async Task<FocusAPI.Models.User> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            return new FocusAPI.Models.User { Id = query.Id };
        }
    }
}