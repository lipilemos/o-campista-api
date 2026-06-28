using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories
{
    public class MensagemChatRepository : IMensagemChatRepository
    {
        private readonly CampistaDbContext _context;

        public MensagemChatRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<MensagemChat> SalvarAsync(MensagemChat mensagem)
        {
            await _context.MensagensChat.AddAsync(mensagem);
            await _context.SaveChangesAsync();
            return mensagem;
        }

        public async Task<List<MensagemChat>> ObterHistoricoAsync(long campingId, int limite, DateTime? antes)
        {
            var janela = DateTime.UtcNow.AddHours(-24);

            var query = _context.MensagensChat
                .Include(m => m.Usuario)
                .Where(m => m.CampingId == campingId && m.DataEnvio >= janela);

            if (antes.HasValue)
                query = query.Where(m => m.DataEnvio < antes.Value);

            return await query
                .OrderByDescending(m => m.DataEnvio)
                .Take(limite)
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();
        }
    }
}
