using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class AchadoPerdidoRepository : IAchadoPerdidoRepository
{
    /// <summary>Itens mais antigos que isso saem do mural (nada é apagado).</summary>
    private const int DiasVisibilidade = 30;

    private readonly CampistaDbContext _context;

    public AchadoPerdidoRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task<AchadoPerdido> CriarAsync(AchadoPerdido item)
    {
        await _context.AchadosPerdidos.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<AchadoPerdido?> ObterPorIdAsync(long id)
    {
        return await _context.AchadosPerdidos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<AchadoPerdido>> ListarPorCampingAsync(
        long campingId, string? tipo, bool incluirResolvidos, int pagina, int limite)
    {
        var limite30Dias = DateTime.UtcNow.AddDays(-DiasVisibilidade);

        var query = _context.AchadosPerdidos
            .AsNoTracking()
            .Include(a => a.Usuario)
            .Where(a => a.CampingId == campingId && a.CriadoEm >= limite30Dias);

        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(a => a.Tipo == tipo);

        if (!incluirResolvidos)
            query = query.Where(a => !a.Resolvido);

        return await query
            .OrderBy(a => a.Resolvido)
            .ThenByDescending(a => a.CriadoEm)
            .Skip((pagina - 1) * limite)
            .Take(limite)
            .ToListAsync();
    }

    public async Task AtualizarAsync(AchadoPerdido item)
    {
        _context.AchadosPerdidos.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeletarAsync(AchadoPerdido item)
    {
        _context.AchadosPerdidos.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteVinculoAtivoAsync(Guid usuarioA, Guid usuarioB)
    {
        var limite30Dias = DateTime.UtcNow.AddDays(-DiasVisibilidade);

        return await _context.AchadosPerdidos
            .AsNoTracking()
            .AnyAsync(a =>
                !a.Resolvido &&
                a.CriadoEm >= limite30Dias &&
                ((a.UsuarioId == usuarioA &&
                  _context.Checkins.Any(c => c.UsuarioId == usuarioB && c.CampingId == a.CampingId)) ||
                 (a.UsuarioId == usuarioB &&
                  _context.Checkins.Any(c => c.UsuarioId == usuarioA && c.CampingId == a.CampingId))));
    }
}
