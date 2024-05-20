using System.Linq.Expressions;
using FocusAPI.Data;
using FocusAPI.Helpers;
using FocusAPI.Models;
using FocusCore.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Repositories;
public class UserRepository : IUserRepository
{
    private readonly FocusAPIContext _context;
    public UserRepository(FocusAPIContext context)
    {
        _context = context;
    }

    public async Task<BaseUser?> GetBaseUserWithItemsByAuth0IdAsync(
        string? auth0Id,
        Expression<Func<User, bool>>[]? wherePredicates = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<User> query = _context.Users.Where(u => u.Auth0Id == auth0Id);

        if (wherePredicates != null)
        {
            foreach (var wherePredicate in wherePredicates)
                query = query.Where(wherePredicate);
        }

        query = query
            .Include(user => user.Islands)
            .Include(user => user.Pets)
            .Include(user => user.Decor)
            .Include(user => user.Badges);

        return await query.Select(user => ProjectionHelper.ProjectToBaseUser(user)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}