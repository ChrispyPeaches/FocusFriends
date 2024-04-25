using System.Linq.Expressions;
using FocusCore.Models;

namespace FocusCore.Extensions;

public static class FriendshipExtensions
{
    public static Expression<Func<BaseFriendship, bool>> GetFriendsFilter(Guid userId)
    {
        return f => (f.UserId == userId || f.FriendId == userId) && f.Status == 1;
    }

    public static Expression<Func<BaseFriendship, bool>> GetFriendsFilter(BaseUser user)
    {
        return f => (f.UserId == user.Id || f.FriendId == user.Id) && f.Status == 1;
    }
}

