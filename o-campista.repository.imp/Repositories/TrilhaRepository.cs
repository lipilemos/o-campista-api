using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class TrilhaRepository : ITrilhaRepository
    {
        private readonly CampistaDbContext _context;

        public TrilhaRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.UsuarioTrilhas
                .CountAsync(x => x.UsuarioId == usuarioId && x.Concluida);
        }

        public async Task<int> ContarCriadasPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Trilhas
                .CountAsync(t => t.CriadorId == usuarioId);
        }

        public async Task<IEnumerable<Trilha>> ObterPorCampingAsync(long campingId)
        {
            return await _context.Trilhas
                .Where(t => t.CampingId == campingId)
                .Include(t => t.Pontos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Trilha?> ObterPorIdComPontosAsync(long trilhaId)
        {
            return await _context.Trilhas
                .Where(t => t.Id == trilhaId)
                .Include(t => t.Pontos.OrderBy(p => p.Ordem))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Trilha> CriarAsync(Trilha trilha)
        {
            await _context.Trilhas.AddAsync(trilha);
            await _context.SaveChangesAsync();

            return await _context.Trilhas
                .Where(t => t.Id == trilha.Id)
                .Include(t => t.Pontos.OrderBy(p => p.Ordem))
                .AsNoTracking()
                .FirstAsync();
        }

        public async Task<IEnumerable<Trilha>> ObterIndependentesAsync()
        {
            return await _context.Trilhas
                .Where(t => t.CampingId == null)
                .Include(t => t.Pontos.OrderBy(p => p.Ordem))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AtualizarMediaAvaliacaoAsync(long trilhaId, double media)
        {
            await _context.Trilhas
                .Where(t => t.Id == trilhaId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.AvaliacaoMedia, media));
        }
    }
}
