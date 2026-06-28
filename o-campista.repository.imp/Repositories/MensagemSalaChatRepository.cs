using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class MensagemSalaChatRepository : IMensagemSalaChatRepository
    {
        private readonly CampistaDbContext _context;

        public MensagemSalaChatRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<MensagemSalaChat> SalvarAsync(MensagemSalaChat mensagem)
        {
            await _context.MensagensSalaChat.AddAsync(mensagem);
            await _context.SaveChangesAsync();
            return mensagem;
        }

        public async Task<List<MensagemSalaChat>> ObterHistoricoAsync(long salaId, int skip, int take)
        {
            return await _context.MensagensSalaChat
                .Include(m => m.Usuario)
                .Where(m => m.SalaId == salaId)
                .OrderByDescending(m => m.DataEnvio)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();
        }

        public async Task<Dictionary<long, int>> ContarNaoLidasAsync(Guid usuarioId)
        {
            var membros = await _context.SalaChatMembros
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            var resultado = new Dictionary<long, int>();

            foreach (var membro in membros)
            {
                var query = _context.MensagensSalaChat
                    .Where(m => m.SalaId == membro.SalaId && m.UsuarioId != usuarioId);

                if (membro.UltimaMensagemLidaId.HasValue)
                    query = query.Where(m => m.Id > membro.UltimaMensagemLidaId.Value);

                var count = await query.CountAsync();
                if (count > 0)
                    resultado[membro.SalaId] = count;
            }

            return resultado;
        }

        public async Task AtualizarUltimaMensagemLidaAsync(long salaId, Guid usuarioId, long ultimaMensagemId)
        {
            var membro = await _context.SalaChatMembros
                .FirstOrDefaultAsync(m => m.SalaId == salaId && m.UsuarioId == usuarioId);

            if (membro is not null)
            {
                membro.UltimaMensagemLidaId = ultimaMensagemId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<long?> ObterUltimaMensagemIdAsync(long salaId)
        {
            return await _context.MensagensSalaChat
                .Where(m => m.SalaId == salaId)
                .OrderByDescending(m => m.Id)
                .Select(m => (long?)m.Id)
                .FirstOrDefaultAsync();
        }
    }
}
