using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class PostRepository : IPostRepository
{
    private readonly CampistaDbContext _context;

    public PostRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task<PostViagem> CriarAsync(PostViagem post)
    {
        await _context.PostsViagem.AddAsync(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<PostViagem?> ObterPorIdAsync(long id)
    {
        return await _context.PostsViagem
            .Include(p => p.Usuario)
            .Include(p => p.Camping)
            .Include(p => p.Trilha)
            .Include(p => p.Curtidas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task DeletarAsync(long id)
    {
        var post = await _context.PostsViagem.FindAsync(id);
        if (post is not null)
        {
            _context.PostsViagem.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CurtirAsync(CurtidaPost curtida)
    {
        await _context.CurtidasPost.AddAsync(curtida);
        await _context.SaveChangesAsync();
    }

    public async Task DescurtirAsync(long postId, Guid usuarioId)
    {
        var curtida = await _context.CurtidasPost
            .FirstOrDefaultAsync(c => c.PostId == postId && c.UsuarioId == usuarioId);
        if (curtida is not null)
        {
            _context.CurtidasPost.Remove(curtida);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> JaCurtiuAsync(long postId, Guid usuarioId)
    {
        return await _context.CurtidasPost
            .AnyAsync(c => c.PostId == postId && c.UsuarioId == usuarioId);
    }

    public async Task<int> ContarCurtidasAsync(long postId)
    {
        return await _context.CurtidasPost.CountAsync(c => c.PostId == postId);
    }
}
