using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetDefaultItems
    {
        internal class Query : IRequest<Result> { }

        internal class Result
        {
            public Island? Island { get; set; }
            public Pet? Pet { get; set; }
            public Decor? Decor { get; set; }
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
                Island? island = await GetInitialIslandQuery().FirstOrDefaultAsync(cancellationToken);
                Pet? pet = await GetInitialPetQuery().FirstOrDefaultAsync(cancellationToken);
                Decor? decorItem = await GetInitialDecorQuery().FirstOrDefaultAsync(cancellationToken);

                return new Result
                {
                    Island = island,
                    Pet = pet,
                    Decor = decorItem
                };
            }

            private IQueryable<Island> GetInitialIslandQuery()
            {
                return _context.Islands
                    .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
            }

            private IQueryable<Pet> GetInitialPetQuery()
            {
                return _context.Pets
                    .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
            }

            private IQueryable<Decor> GetInitialDecorQuery()
            {
                return _context.Decor;
            }


        }
    }
}
