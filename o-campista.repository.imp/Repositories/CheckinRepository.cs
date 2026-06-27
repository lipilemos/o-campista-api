using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;
public class CheckinRepository : ICheckinRepository
{
    private readonly CampistaDbContext _context;

    public CheckinRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(Checkin checkin)
    {
        await _context.AddAsync(checkin);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> JaExisteHojeAsync(
        Guid usuarioId,
        long campingId)
    {
        var hoje = DateTime.UtcNow.Date;

        return await _context.Set<Checkin>()
            .AnyAsync(x =>
                x.UsuarioId == usuarioId &&
                x.CampingId == campingId &&
                x.CriadoEm.Date == hoje);
    }

    public async Task<int>ObterQuantidadeCheckinsUsuarioAsync(Guid usuarioId)
    {
        return await _context.Set<Checkin>()
            .CountAsync(x =>
                x.UsuarioId == usuarioId);
    }
    public async Task<int> ContarPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Checkins
            .CountAsync(x =>
                x.UsuarioId == usuarioId);
    }

    public async Task<List<Checkin>> ObtenerHistoricoAsync(Guid usuarioId)
    {
        return await _context.Set<Checkin>()
            .Where(x => x.UsuarioId == usuarioId)
            .Include(x => x.Camping)
            .ThenInclude(x=>x.Fotos)
            .OrderByDescending(x => x.CriadoEm)
            .ToListAsync();
    }
}
