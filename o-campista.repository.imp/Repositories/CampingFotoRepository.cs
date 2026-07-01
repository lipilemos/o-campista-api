using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class CampingFotoRepository : ICampingFotoRepository
    {
        private readonly CampistaDbContext _context;

        public CampingFotoRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CampingFoto>> ObterPorCampingAsync(long campingId)
        {
            return await _context.CampingFotos
                .Where(f => f.CampingId == campingId)
                .OrderByDescending(f => f.Principal)
                .ThenBy(f => f.Ordem)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
