using System.Linq.Expressions;
using FocusAPI.Models;
using FocusCore.Models;

namespace FocusAPI.Repositories;
public interface IUserRepository
{
    public Task<BaseUser?> GetBaseUserWithItemsByAuth0IdAsync(
        string? auth0Id,
        Expression<Func<User, bool>>[]? wherePredicates = null,
        CancellationToken cancellationToken = default
    );

    public Task SaveChangesAsync();
}