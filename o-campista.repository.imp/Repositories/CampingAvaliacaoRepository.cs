using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class CampingAvaliacaoRepository : ICampingAvaliacaoRepository
    {
        private readonly CampistaDbContext _context;

        public CampingAvaliacaoRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task CriarAsync(CampingAvaliacao avaliacao)
        {
            avaliacao.CriadoEm = DateTime.UtcNow;
            await _context.AddAsync(avaliacao);
            await _context.SaveChangesAsync();
        }

        public async Task<CampingAvaliacao?> ObterPorIdAsync(long id)
        {
            return await _context.Set<CampingAvaliacao>()
                .Include(x => x.Usuario)
                .Include(x => x.Camping)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CampingAvaliacao?> ObterPorCheckinAsync(long checkinId)
        {
            return await _context.Set<CampingAvaliacao>()
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.CheckinId == checkinId);
        }

        public async Task<List<CampingAvaliacao>> ObterPorCampingAsync(long campingId)
        {
            return await _context.Set<CampingAvaliacao>()
                .Where(x => x.CampingId == campingId)
                .Include(x => x.Usuario)
                .OrderByDescending(x => x.CriadoEm)
                .ToListAsync();
        }

        public async Task<List<CampingAvaliacao>> ObterPorCampingEUsuarioAsync(long campingId, Guid usuarioId, long? checkinId = null)
        {
            var query = _context.Set<CampingAvaliacao>()
                .Where(x => x.CampingId == campingId && x.UsuarioId == usuarioId);

            if (checkinId.HasValue)
                query = query.Where(x => x.CheckinId == checkinId.Value);

            return await query
                .Include(x => x.Usuario)
                .OrderByDescending(x => x.CriadoEm)
                .ToListAsync();
        }

        public async Task<List<CampingAvaliacao>> ObterTodosAsync()
        {
            return await _context.Set<CampingAvaliacao>()
                .Include(x => x.Usuario)
                .OrderByDescending(x => x.CriadoEm)
                .ToListAsync();
        }

        public async Task<int> ContarPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Set<CampingAvaliacao>()
                .CountAsync(x => x.UsuarioId == usuarioId);
        }

        public async Task AtualizarAsync(CampingAvaliacao avaliacao)
        {
            _context.Update(avaliacao);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(long id)
        {
            var avaliacao = await _context.Set<CampingAvaliacao>().FindAsync(id);
            if (avaliacao != null)
            {
                _context.Remove(avaliacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CampingAvaliacao>> ObterPorTrilhaAsync(long trilhaId)
        {
            return await _context.Set<CampingAvaliacao>()
                .Where(x => x.TrilhaId == trilhaId)
                .Include(x => x.Usuario)
                .OrderByDescending(x => x.CriadoEm)
                .ToListAsync();
        }

        public async Task<double> ObterMediaNotasPorTrilhaAsync(long trilhaId)
        {
            var avaliacoes = await _context.Set<CampingAvaliacao>()
                .Where(x => x.TrilhaId == trilhaId)
                .ToListAsync();

            return avaliacoes.Count > 0 ? avaliacoes.Average(a => a.Nota) : 0;
        }
    }
}
