using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetUserDecor
    {
        internal class Query : IRequest<Result> 
        {
            public Guid UserId { get; set; }
            public Guid? selectedDecorId { get; set; }
        }

        internal class Result
        {
            public List<DecorItem> Decor { get; set; }
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
                List<DecorItem> userDecor = new List<DecorItem>();

                Guid userSelectedDecorId = Guid.Empty;
                if (query.selectedDecorId != null)
                {
                    userSelectedDecorId = query.selectedDecorId.Value;
                }

                userDecor = await _context.UserDecor
                    .Include(d => d.Decor)
                    .Where(d => d.UserId == query.UserId)
                    .Select(d =>
                    new DecorItem
                    {
                        DecorId = d.DecorId,
                        DecorName = d.Decor.Name,
                        DecorPicture = d.Decor.Image,

                        // Determine if decor is currently selected
                        isSelected = userSelectedDecorId == d.DecorId
                    })
                    .ToListAsync();

                return new Result
                {
                    Decor = userDecor
                };
            }
        }
    }
}
