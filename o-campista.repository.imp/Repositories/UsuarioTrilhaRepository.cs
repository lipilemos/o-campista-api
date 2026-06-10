using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using o_campista.entities.Entities.o_campista.entities.Entities;

namespace o_campista.repository.imp.Repositories
{
    public class UsuarioTrilhaRepository: IUsuarioTrilhaRepository
    {
        private readonly CampistaDbContext _context;

        public UsuarioTrilhaRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioTrilha?> ObterAsync(Guid usuarioId,long trilhaId)
        {
            return await _context.UsuarioTrilhas
                .FirstOrDefaultAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.TrilhaId == trilhaId);
        }

        public async Task CriarAsync(UsuarioTrilha usuarioTrilha)
        {
            await _context.UsuarioTrilhas
                .AddAsync(usuarioTrilha);

            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(UsuarioTrilha usuarioTrilha)
        {
            _context.UsuarioTrilhas
                .Update(usuarioTrilha);

            await _context.SaveChangesAsync();
        }

        public async Task<int>ContarConcluidasPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.UsuarioTrilhas
                .CountAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.Concluida);
        }
    }
}
