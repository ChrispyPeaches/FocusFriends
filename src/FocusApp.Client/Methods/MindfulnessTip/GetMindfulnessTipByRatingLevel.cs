using FluentValidation;
using FocusApp.Shared.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
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
        private readonly FocusAppContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(FocusAppContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Shared.Models.MindfulnessTip?> Handle(Query query, CancellationToken cancellationToken)
        {
            Shared.Models.MindfulnessTip? resultingTip = null;
            try
            {
                int indexOfTipToRetrieve = await GetRandomTipIndex(query, cancellationToken);

                resultingTip = await GetTipByIndex(query, indexOfTipToRetrieve, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting mindfulness tip by rating level");
            }

            return resultingTip;
        }

        private async Task<int> GetRandomTipIndex(Query query, CancellationToken cancellationToken)
        {
            int randomInt = new Random().Next();

            int numberOfTips = await _context.MindfulnessTips
                .Where(tip => tip.SessionRatingLevel == query.RatingLevel.ToString())
                .CountAsync(cancellationToken);

            return randomInt % numberOfTips;

        }

        private async Task<Shared.Models.MindfulnessTip?> GetTipByIndex(
            Query query,
            int indexOfTipToRetrieve,
            CancellationToken cancellationToken)
        {
            return await _context.MindfulnessTips
                .Where(tip => tip.SessionRatingLevel == query.RatingLevel.ToString())
                .Skip(indexOfTipToRetrieve)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}