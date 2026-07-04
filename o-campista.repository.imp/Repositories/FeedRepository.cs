using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class FeedRepository : IFeedRepository
{
    private readonly CampistaDbContext _context;

    public FeedRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task CriarAtividadeAsync(AtividadeFeed atividade)
    {
        await _context.AtividadesFeed.AddAsync(atividade);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AtividadeFeed>> ObterAtividadesPorUsuariosAsync(IEnumerable<Guid> usuarioIds, int skip, int take)
    {
        return await _context.AtividadesFeed
            .Include(a => a.Usuario)
            .Where(a => usuarioIds.Contains(a.UsuarioId) && a.Visivel)
            .OrderByDescending(a => a.CriadoEm)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<AtividadeFeed>> ObterAtividadesPublicasAsync(int skip, int take)
    {
        var usuariosPrivados = await _context.ConfiguracaoPrivacidades
            .Where(c => !c.PerfilPublico)
            .Select(c => c.UsuarioId)
            .ToListAsync();

        return await _context.AtividadesFeed
            .Include(a => a.Usuario)
            .Where(a => a.Visivel && !usuariosPrivados.Contains(a.UsuarioId))
            .OrderByDescending(a => a.CriadoEm)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Checkin>> ObterCheckinsPorIdsAsync(IEnumerable<long> ids)
    {
        return await _context.Checkins
            .Include(c => c.Camping)
                .ThenInclude(c => c!.Fotos)
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }

    public async Task<List<CampingAvaliacao>> ObterAvaliacoesPorIdsAsync(IEnumerable<long> ids)
    {
        return await _context.CampingAvaliacoes
            .Include(a => a.Camping)
            .Include(a => a.Trilha)
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();
    }

    public async Task<List<Conquista>> ObterConquistasPorIdsAsync(IEnumerable<long> ids)
    {
        return await _context.Conquistas
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }

    public async Task<List<PostViagem>> ObterPostsPorIdsAsync(IEnumerable<long> ids)
    {
        return await _context.PostsViagem
            .Include(p => p.Camping)
            .Include(p => p.Trilha)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<List<Trilha>> ObterTrilhasPorIdsAsync(IEnumerable<long> ids)
    {
        return await _context.Trilhas
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<Dictionary<long, int>> ObterCurtidasTotaisPorPostIdsAsync(IEnumerable<long> postIds)
    {
        return await _context.CurtidasPost
            .Where(c => postIds.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Total);
    }

    public async Task<List<long>> ObterPostsCurtidosPeloUsuarioAsync(Guid usuarioId, IEnumerable<long> postIds)
    {
        return await _context.CurtidasPost
            .Where(c => c.UsuarioId == usuarioId && postIds.Contains(c.PostId))
            .Select(c => c.PostId)
            .ToListAsync();
    }

    public async Task<Dictionary<long, int>> ObterComentariosTotaisPorPostIdsAsync(IEnumerable<long> postIds)
    {
        return await _context.ComentariosPost
            .Where(c => postIds.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Total);
    }
}
