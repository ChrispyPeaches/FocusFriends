using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetDefaultPetAndIsland
    {
        internal class Query : IRequest<Result> { }

        internal class Result
        {
            public Island? Island { get; set; }
            public Pet? Pet { get; set; }
        }

        internal class Handler : IRequestHandler<Query, Result>
        {
            FocusAppContext _localContext;

            public Handler(FocusAppContext localContext)
            {
                _localContext = localContext;
            }

            public async Task<Result> Handle(
                Query query,
                CancellationToken cancellationToken = default)
            {
                Island? island = await GetInitialIslandQuery().FirstOrDefaultAsync(cancellationToken);
                Pet? pet = await GetInitialPetQuery().FirstOrDefaultAsync(cancellationToken);

                return new Result
                {
                    Island = island,
                    Pet = pet
                };
            }

            private IQueryable<Island> GetInitialIslandQuery()
            {
                return _localContext.Islands
                    .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
            }

            private IQueryable<Pet> GetInitialPetQuery()
            {
                return _localContext.Pets
                    .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
            }
        }
    }
}
