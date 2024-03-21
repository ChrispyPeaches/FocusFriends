using FocusAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusAPI.Methods.Sync
{
    public class SyncMindfulnessTips
    {
        public class Validator : AbstractValidator<SyncMindfulnessTipsQuery>
        {
            public Validator()
            {
                RuleFor(query => query.MindfulnessTipIds)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<SyncMindfulnessTipsQuery, SyncMindfulnessTipsResponse>
        {
            private readonly FocusContext _context;

            public Handler(FocusContext context)
            {
                _context = context;
            }

            public async Task<SyncMindfulnessTipsResponse> Handle(
                SyncMindfulnessTipsQuery query,
                CancellationToken cancellationToken)
            {
                List<Guid> serverIds = await GetServerMindfulnessTipIds(cancellationToken);
                IList<Guid> mobileIds = query.MindfulnessTipIds;

                List<FocusCore.Models.BaseMindfulnessTip> missingTips = new();
                if (serverIds.Count != 0)
                {
                    List<Guid> missingTipIds = FindTipIdsToAddToMobileDatabase(serverIds, mobileIds);

                    if (missingTipIds.Count > 0)
                    {
                        missingTips = await GetMissingTipsFromServer(missingTipIds, cancellationToken);
                    }
                }

                return new SyncMindfulnessTipsResponse()
                {
                    MissingTips = missingTips
                };
            }

            private async Task<List<Guid>> GetServerMindfulnessTipIds(CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.MindfulnessTips
                        .Select(tip => tip.Id)
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from API and/or local db.", ex);
                }
            }

            private static List<Guid> FindTipIdsToAddToMobileDatabase(
                List<Guid> serverIds,
                IList<Guid> mobileIds)
            {
                List<Guid> missingTipIds;
                if (mobileIds.Any())
                {
                    missingTipIds = serverIds
                        .Except(mobileIds)
                        .ToList();
                }
                else
                {
                    missingTipIds = serverIds;
                }

                return missingTipIds;
            }

            private async Task<List<FocusCore.Models.BaseMindfulnessTip>> GetMissingTipsFromServer(
                List<Guid> missingTipIds,
                CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.MindfulnessTips
                        .Where(tip => missingTipIds.Contains(tip.Id))
                        .Select(tip => new FocusCore.Models.BaseMindfulnessTip()
                        {
                            Id = tip.Id,
                            Title = tip.Title,
                            Content = tip.Content,
                            SessionRatingLevel = tip.SessionRatingLevel
                        })
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from API and/or local db.", ex);
                }
            }   
        }

    }
}
