using System.Net;
using FocusCore.Queries.User;
using FocusCore.Models;
using MediatR;
using FocusAPI.Data;
using FocusAPI.Helpers;
using FocusCore.Responses;
using FocusCore.Responses.User;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class GetUser
{
    public class Handler : IRequestHandler<GetUserQuery, MediatrResultWrapper<GetUserResponse>>
    {
        IFocusAPIContext _apiContext;
        public Handler(IFocusAPIContext apiContext) 
        {
            _apiContext = apiContext;
        }

        public async Task<MediatrResultWrapper<GetUserResponse>> Handle(
            GetUserQuery query,
            CancellationToken cancellationToken = default)
        {
            BaseUser? user = await GetUser(query, cancellationToken);

            if (user != null)
            {
                ExtractUserRelationIds(
                    user,
                    out List<Guid> userIslandIds,
                    out List<Guid> userPetIds,
                    out List<Guid> userDecorIds,
                    out List<Guid> userBadgeIds);

                return new()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Data = new GetUserResponse
                    {
                        User = user,
                        UserIslandIds = userIslandIds,
                        UserPetIds = userPetIds,
                        UserDecorIds = userDecorIds,
                        UserBadgeIds = userBadgeIds
                    }
                };
            }
            else
            {
                return new()
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = $"User not found with Auth0Id: {query.Auth0Id}"
                };
            }
        }

        private async Task<BaseUser?> GetUser(
            GetUserQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _apiContext.Users
                    .Where(u => u.Auth0Id == query.Auth0Id)
                    .Include(user => user.Islands)
                    .Include(user => user.Pets)
                    .Include(user => user.Decor)
                    .Include(user => user.Badges)
                    .Select(user => ProjectionHelper.ProjectToBaseUser(user))
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Error getting user: {e.Message}");
            }
        }


        /// <summary>
        /// Extract the item relation ids from the user object to optimize the response size
        /// </summary>
        private static void ExtractUserRelationIds(
            BaseUser user,
            out List<Guid> userIslandIds,
            out List<Guid> userPetIds,
            out List<Guid> userDecorIds,
            out List<Guid> userBadgeIds)
        {
            userIslandIds = user.Islands?.Select(ui => ui.IslandId).ToList() ?? new();
            userPetIds = user.Pets?.Select(up => up.PetId).ToList() ?? new();
            userDecorIds = user.Decor?.Select(ud => ud.DecorId).ToList() ?? new();
            userBadgeIds = user.Badges?.Select(ub => ub.BadgeId).ToList() ?? new();

            user.Islands = new List<BaseUserIsland>();
            user.Pets = new List<BaseUserPet>();
            user.Decor = new List<BaseUserDecor>();
            user.Badges = new List<BaseUserBadge>();
        }
    }
}