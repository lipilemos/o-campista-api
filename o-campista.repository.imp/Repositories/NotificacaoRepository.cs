using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class NotificacaoRepository : INotificacaoRepository
{
    private readonly CampistaDbContext _context;

    public NotificacaoRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(Notificacao notificacao)
    {
        await _context.Notificacoes.AddAsync(notificacao);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notificacao>> ObterPorDestinatarioAsync(Guid destinatarioId, int pagina, int limite)
    {
        limite = Math.Clamp(limite, 1, 50);
        var skip = (pagina - 1) * limite;

        return await _context.Notificacoes
            .Where(n => n.DestinatarioId == destinatarioId)
            .Include(n => n.Remetente)
            .OrderByDescending(n => n.CriadoEm)
            .Skip(skip)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<int> ContarNaoLidasAsync(Guid destinatarioId)
    {
        return await _context.Notificacoes
            .CountAsync(n => n.DestinatarioId == destinatarioId && !n.Lida);
    }

    public async Task MarcarTodasComoLidasAsync(Guid destinatarioId)
    {
        await _context.Notificacoes
            .Where(n => n.DestinatarioId == destinatarioId && !n.Lida)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.Lida, true));
    }
}
