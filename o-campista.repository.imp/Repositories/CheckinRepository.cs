using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

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

    public async Task<int> ObterQuantidadeCheckinsUsuarioAsync(Guid usuarioId)
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
            .ThenInclude(x => x.Fotos)
            .Include(x => x.Trilha)
            .OrderByDescending(x => x.CriadoEm)
            .ToListAsync();
    }

    public async Task<bool> TemCheckinNasUltimas24hAsync(Guid usuarioId, long campingId)
    {
        var limite = DateTime.UtcNow.AddHours(-24);
        return await _context.Checkins
            .AnyAsync(c =>
                c.UsuarioId == usuarioId &&
                c.CampingId == campingId &&
                c.CriadoEm >= limite);
    }

    public async Task<int> ContarCheckinsUltimas24hAsync(long campingId)
    {
        var limite = DateTime.UtcNow.AddHours(-24);
        return await _context.Checkins
            .Where(c => c.CampingId == campingId && c.CriadoEm >= limite)
            .Select(c => c.UsuarioId)
            .Distinct()
            .CountAsync();
    }

    public async Task<bool> JaExisteTrilhaHojeAsync(Guid usuarioId, long trilhaId)
    {
        var hoje = DateTime.UtcNow.Date;
        return await _context.Checkins
            .AnyAsync(x =>
                x.UsuarioId == usuarioId &&
                x.TrilhaId == trilhaId &&
                x.CriadoEm.Date == hoje);
    }

    public async Task<int> ContarCheckinsUltimas24hTrilhaAsync(long trilhaId)
    {
        var limite = DateTime.UtcNow.AddHours(-24);
        return await _context.Checkins
            .Where(c => c.TrilhaId == trilhaId && c.CriadoEm >= limite)
            .Select(c => c.UsuarioId)
            .Distinct()
            .CountAsync();
    }

    public async Task<int> ContarCheckinsTrilhaPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.Checkins
            .CountAsync(x => x.UsuarioId == usuarioId && x.TrilhaId != null);
    }

    public async Task<int> ContarTotalCheckinsCampingAsync(long campingId)
    {
        return await _context.Checkins
            .Where(c => c.CampingId == campingId)
            .Select(c => c.UsuarioId)
            .CountAsync();
    }

    public async Task<int> ContarTotalCheckinsTrilhaAsync(long trilhaId)
    {
        return await _context.Checkins
            .Where(c => c.TrilhaId == trilhaId)
            .Select(c => c.UsuarioId)
            .CountAsync();
    }

    public async Task<Dictionary<long, StatusOcupacaoResponse>> ObterStatusOcupacaoTodosAsync()
    {
        var limite = DateTime.UtcNow.AddHours(-6);

        var contagens = await _context.Checkins
            .Where(c => c.CampingId != null && c.Ocupacao != null && c.CriadoEm >= limite)
            .GroupBy(c => new { c.CampingId, c.Ocupacao })
            .Select(g => new
            {
                CampingId = g.Key.CampingId!.Value,
                Nivel = g.Key.Ocupacao!,
                Contagem = g.Count(),
                AtualizadoEm = g.Max(c => c.CriadoEm)
            })
            .ToListAsync();

        return contagens
            .GroupBy(x => x.CampingId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var moda = g.OrderByDescending(x => x.Contagem).First();
                    return new StatusOcupacaoResponse
                    {
                        Nivel = moda.Nivel,
                        AtualizadoEm = moda.AtualizadoEm
                    };
                });
    }
}
