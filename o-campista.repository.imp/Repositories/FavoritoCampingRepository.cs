using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class FavoritoCampingRepository : IFavoritoCampingRepository
{
    private readonly CampistaDbContext _context;

    public FavoritoCampingRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(UsuarioCampingFavorito favorito)
    {
        await _context.UsuarioCampingFavoritos.AddAsync(favorito);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid usuarioId, long campingId)
    {
        var favorito = await _context.UsuarioCampingFavoritos
            .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.CampingId == campingId);

        if (favorito is not null)
        {
            _context.UsuarioCampingFavoritos.Remove(favorito);
            await _context.SaveChangesAsync();
        }
    }

    public Task<bool> ExisteAsync(Guid usuarioId, long campingId)
    {
        return _context.UsuarioCampingFavoritos
            .AnyAsync(f => f.UsuarioId == usuarioId && f.CampingId == campingId);
    }

    public async Task<List<Camping>> ObterPorUsuarioAsync(Guid usuarioId)
    {
        return await _context.UsuarioCampingFavoritos
            .Where(f => f.UsuarioId == usuarioId)
            .Include(f => f.Camping)
                .ThenInclude(c => c.Fotos)
            .Include(f => f.Camping)
                .ThenInclude(c => c.Recursos)
                    .ThenInclude(r => r.Recurso)
            .OrderByDescending(f => f.CriadoEm)
            .Select(f => f.Camping)
            .ToListAsync();
    }
}
