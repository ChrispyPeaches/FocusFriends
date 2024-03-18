using FluentValidation;
using FocusApp.Shared.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static FocusCore.Extensions.MindfulnessTipExtensions;

namespace FocusApp.Client.Methods.MindfulnessTip;
public class GetMindfulnessTipByRatingLevel
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.RatingLevel)
                .NotNull()
                .NotEmpty()
                .NotEqual(FocusSessionRating.None);
        }
    }

    public class Query : IRequest<Shared.Models.MindfulnessTip?>
    {
        public FocusSessionRating RatingLevel { get; set; }
    }

    public class Handler : IRequestHandler<Query, Shared.Models.MindfulnessTip?>
    {
        FocusAppContext _context;
        public Handler(FocusAppContext context)
        {
            _context = context;
        }

        public async Task<Shared.Models.MindfulnessTip?> Handle(Query query, CancellationToken cancellationToken)
        {
            var resultingTip = await _context.MindfulnessTips
                .FirstOrDefaultAsync(tip => tip.SessionRatingLevel == query.RatingLevel.ToString(), cancellationToken);

            return resultingTip;
        }
    }
}