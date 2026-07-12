using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly CampistaDbContext _context;

        public UsuarioRepository(
            CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(x => x.UsuarioConquistas)
                .Include(x => x.UsuarioPresentes)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Usuario?> ObterPorIdComDetalhesAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(x => x.UsuarioConquistas)
                    .ThenInclude(x => x.Conquista)

                .Include(x => x.UsuarioPresentes)
                    .ThenInclude(x => x.Presente)

                .Include(x => x.Checkins)
                    .ThenInclude(x => x.Camping)

                .Include(x => x.Checkins)
                    .ThenInclude(x => x.Trilha)

                .Include(x => x.UsuarioTrilhas)
                    .ThenInclude(x => x.Trilha)

                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Usuario?> ObterPorEmailAsync(
            string email)
        {
            return await _context.Usuarios
                .Include(x => x.UsuarioConquistas)
                    .ThenInclude(x => x.Conquista)
                .Include(x => x.UsuarioPresentes)
                    .ThenInclude(x => x.Presente)
                .Include(x => x.UsuarioTrilhas)
                    .ThenInclude(x => x.Trilha)
                .Include(x => x.Checkins)
                    .ThenInclude(x => x.Camping)
                .FirstOrDefaultAsync(
                    x => x.Email == email);
        }

        public async Task<bool> EmailExisteAsync(
            string email)
        {
            return await _context.Usuarios
                .AnyAsync(x => x.Email == email);
        }

        public async Task AdicionarAsync(
            Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);

            await _context.SaveChangesAsync();
        }
    }
}
