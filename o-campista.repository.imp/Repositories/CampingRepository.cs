using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.imp.Repositories
{
    public class CampingRepository : ICampingRepository
    {
        private readonly CampistaDbContext _context;

        public CampingRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Camping>> ObterCampingsMapaAsync()
        {
            return await _context.Campings
                .AsNoTracking()
                .Include(x => x.Fotos)
                .Include(x => x.Recursos)
                    .ThenInclude(x => x.Recurso)
                .ToListAsync();
        }
    }
}
