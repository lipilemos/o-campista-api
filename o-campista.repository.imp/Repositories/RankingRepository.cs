using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class RankingRepository : IRankingRepository
{
    private readonly CampistaDbContext _context;

    public RankingRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task<List<(Guid Id, string Nome, string? FotoPerfil, int Nivel, int Xp, int TotalCheckins)>>
        ObterRankingGlobalAsync(int skip, int take)
    {
        var checkinsContagem = await _context.Checkins
            .Where(c => c.CampingId != null)
            .GroupBy(c => c.UsuarioId)
            .Select(g => new { UsuarioId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

        var usuarios = await _context.Usuarios
            .Where(u => u.Ativo)
            .OrderByDescending(u => u.XP)
            .ThenByDescending(u => u.Nivel)
            .Skip(skip)
            .Take(take)
            .Select(u => new { u.Id, u.Nome, u.FotoPerfil, u.Nivel, u.XP })
            .ToListAsync();

        return usuarios.Select(u => (
            u.Id,
            u.Nome,
            u.FotoPerfil,
            u.Nivel,
            u.XP,
            checkinsContagem.GetValueOrDefault(u.Id, 0)
        )).ToList();
    }

    public async Task<List<(Guid Id, string Nome, string? FotoPerfil, int Nivel, int Xp, int TotalCheckins)>>
        ObterRankingPorIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return [];

        var checkinsContagem = await _context.Checkins
            .Where(c => c.CampingId != null && idList.Contains(c.UsuarioId))
            .GroupBy(c => c.UsuarioId)
            .Select(g => new { UsuarioId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.UsuarioId, x => x.Total);

        var usuarios = await _context.Usuarios
            .Where(u => u.Ativo && idList.Contains(u.Id))
            .OrderByDescending(u => u.XP)
            .ThenByDescending(u => u.Nivel)
            .Select(u => new { u.Id, u.Nome, u.FotoPerfil, u.Nivel, u.XP })
            .ToListAsync();

        return usuarios.Select(u => (
            u.Id,
            u.Nome,
            u.FotoPerfil,
            u.Nivel,
            u.XP,
            checkinsContagem.GetValueOrDefault(u.Id, 0)
        )).ToList();
    }

    public async Task<List<(long Id, string Nome, string? Cidade, string? Estado, string? FotoPrincipal, decimal AvaliacaoMedia, int TotalCheckins)>>
        ObterRankingCampingsAsync(int skip, int take)
    {
        var totaisPorCamping = await _context.Checkins
            .Where(c => c.CampingId != null)
            .GroupBy(c => c.CampingId!.Value)
            .Select(g => new { CampingId = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        if (totaisPorCamping.Count == 0) return [];

        var campingIds = totaisPorCamping.Select(x => x.CampingId).ToList();

        var campings = await _context.Campings
            .Include(c => c.Fotos)
            .Where(c => campingIds.Contains(c.Id) && c.Ativo)
            .ToDictionaryAsync(c => c.Id);

        return totaisPorCamping
            .Where(x => campings.ContainsKey(x.CampingId))
            .Select(x =>
            {
                var c = campings[x.CampingId];
                return (
                    c.Id,
                    c.Nome,
                    c.Cidade,
                    c.Estado,
                    c.Fotos.FirstOrDefault()?.Url,
                    c.AvaliacaoMedia,
                    x.Total
                );
            })
            .ToList();
    }
}
