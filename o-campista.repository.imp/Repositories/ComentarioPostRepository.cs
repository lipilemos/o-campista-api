using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class ComentarioPostRepository : IComentarioPostRepository
{
    private readonly CampistaDbContext _context;

    public ComentarioPostRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task<ComentarioPost> CriarAsync(ComentarioPost comentario)
    {
        await _context.ComentariosPost.AddAsync(comentario);
        await _context.SaveChangesAsync();
        return comentario;
    }

    public async Task<ComentarioPost?> ObterPorIdAsync(long id)
    {
        return await _context.ComentariosPost
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<ComentarioPost>> ObterPorPostAsync(long postId, int pagina, int limite)
    {
        limite = Math.Clamp(limite, 1, 100);
        var skip = (pagina - 1) * limite;

        return await _context.ComentariosPost
            .Where(c => c.PostId == postId)
            .Include(c => c.Usuario)
            .OrderByDescending(c => c.CriadoEm)
            .Skip(skip)
            .Take(limite)
            .ToListAsync();
    }

    public async Task DeletarAsync(long id)
    {
        var comentario = await _context.ComentariosPost.FindAsync(id);
        if (comentario is not null)
        {
            _context.ComentariosPost.Remove(comentario);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ContarPorPostAsync(long postId)
    {
        return await _context.ComentariosPost.CountAsync(c => c.PostId == postId);
    }
}
