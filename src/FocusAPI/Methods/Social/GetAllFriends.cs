using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Queries.Shop;
using MediatR;

namespace FocusApi.Methods.Social;
public class GetAllFriends
{
    public class Handler// : IRequestHandler<GetAllShopItemsQuery, List<ShopItem>>
    {
        FocusContext _context;
        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<List<FriendModel>> Handle(GetAllShopItemsQuery query, CancellationToken cancellationToken)
        {
            List<BaseFriendship> friendships = _context.Friends.OfType<BaseFriendship>().ToList();

            var friends = friendships.Select(f => new FriendModel
            {
                FriendUserName = f.Friend.UserName,
                Email = f.Friend.Email,
                ProfilePicture = f.Friend.ProfilePicture
            });

            return friends.ToList();
        }
    }
}