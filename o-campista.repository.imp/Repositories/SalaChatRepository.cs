using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class SalaChatRepository : ISalaChatRepository
    {
        private readonly CampistaDbContext _context;

        public SalaChatRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalaChat>> ObterSalasDoUsuarioAsync(Guid usuarioId)
        {
            return await _context.SalasChat
                .Where(s => s.Membros.Any(m => m.UsuarioId == usuarioId))
                .Include(s => s.Camping)
                .Include(s => s.Mensagens.OrderByDescending(m => m.DataEnvio).Take(1))
                    .ThenInclude(m => m.Usuario)
                .OrderByDescending(s => s.Mensagens.Max(m => (DateTime?)m.DataEnvio) ?? s.CriadoEm)
                .ToListAsync();
        }

        public async Task<SalaChat?> ObterPorIdAsync(long salaId)
        {
            return await _context.SalasChat
                .Include(s => s.Camping)
                .FirstOrDefaultAsync(s => s.Id == salaId);
        }

        public async Task<SalaChat> CriarAsync(SalaChat sala)
        {
            await _context.SalasChat.AddAsync(sala);
            await _context.SaveChangesAsync();
            return sala;
        }

        public async Task<SalaChat?> ObterPorCampingIdAsync(long campingId)
        {
            return await _context.SalasChat
                .FirstOrDefaultAsync(s => s.CampingId == campingId && s.Tipo == "camping");
        }

        public async Task<SalaChat?> ObterPorCodigoConviteAsync(string codigo)
        {
            return await _context.SalasChat
                .FirstOrDefaultAsync(s => s.CodigoConvite == codigo);
        }

        public async Task AdicionarMembroAsync(SalaChatMembro membro)
        {
            await _context.SalaChatMembros.AddAsync(membro);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SalaChatMembro>> ObterMembrosAsync(long salaId)
        {
            return await _context.SalaChatMembros
                .Where(m => m.SalaId == salaId)
                .Include(m => m.Usuario)
                .ToListAsync();
        }

        public async Task<bool> UsuarioEhMembroAsync(long salaId, Guid usuarioId)
        {
            return await _context.SalaChatMembros
                .AnyAsync(m => m.SalaId == salaId && m.UsuarioId == usuarioId);
        }
    }
}
