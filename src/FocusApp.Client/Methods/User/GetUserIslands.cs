using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetUserIslands
    {
        internal class Query : IRequest<Result> 
        {
            public Guid UserId { get; set; }
            public Guid? selectedIslandId { get; set; }
        }

        internal class Result
        {
            public List<IslandItem> Islands { get; set; }
        }

        internal class Handler : IRequestHandler<Query, Result>
        {
            FocusAppContext _context;

            public Handler(FocusAppContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(
                Query query,
                CancellationToken cancellationToken = default)
            {
                List<IslandItem> userIslands = new List<IslandItem>();

                Guid userSelectedIslandId = Guid.Empty;
                if (query.selectedIslandId != null)
                {
                    userSelectedIslandId = query.selectedIslandId.Value;
                }

                userIslands = await _context.UserIslands
                    .Include(i => i.Island)
                    .Where(i => i.UserId == query.UserId)
                    .Select(i =>
                    new IslandItem
                    {
                        IslandId = i.IslandId,
                        IslandName = i.Island.Name,
                        IslandPicture = i.Island.Image,

                        // Determine if island is currently selected
                        isSelected = userSelectedIslandId == i.IslandId
                    })
                    .ToListAsync();

                return new Result
                {
                    Islands = userIslands
                };
            }
        }
    }
}
