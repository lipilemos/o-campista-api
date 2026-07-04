using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;

namespace o_campista.repository.imp.Repositories;

public class SocialRepository : ISocialRepository
{
    private readonly CampistaDbContext _context;

    public SocialRepository(CampistaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EstaSeguidoAsync(Guid seguidorId, Guid seguidoId)
    {
        return await _context.Seguidores
            .AnyAsync(s => s.SeguidorId == seguidorId && s.SeguidoId == seguidoId);
    }

    public async Task<bool> SegueMutuamenteAsync(Guid usuarioAId, Guid usuarioBId)
    {
        var aSegueB = await EstaSeguidoAsync(usuarioAId, usuarioBId);
        if (!aSegueB) return false;
        return await EstaSeguidoAsync(usuarioBId, usuarioAId);
    }

    public async Task SegueAsync(Seguidor seguidor)
    {
        await _context.Seguidores.AddAsync(seguidor);
        await _context.SaveChangesAsync();
    }

    public async Task PararDeSegueAsync(Guid seguidorId, Guid seguidoId)
    {
        var seguidor = await _context.Seguidores
            .FirstOrDefaultAsync(s => s.SeguidorId == seguidorId && s.SeguidoId == seguidoId);
        if (seguidor is not null)
        {
            _context.Seguidores.Remove(seguidor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Usuario>> ObterSeguidoresAsync(Guid usuarioId)
    {
        return await _context.Seguidores
            .Where(s => s.SeguidoId == usuarioId)
            .Select(s => s.SeguidorUsuario)
            .ToListAsync();
    }

    public async Task<List<Usuario>> ObterSeguindoAsync(Guid usuarioId)
    {
        return await _context.Seguidores
            .Where(s => s.SeguidorId == usuarioId)
            .Select(s => s.SeguidoUsuario)
            .ToListAsync();
    }

    public async Task<int> ContarSeguidoresAsync(Guid usuarioId)
    {
        return await _context.Seguidores.CountAsync(s => s.SeguidoId == usuarioId);
    }

    public async Task<int> ContarSeguindoAsync(Guid usuarioId)
    {
        return await _context.Seguidores.CountAsync(s => s.SeguidorId == usuarioId);
    }

    public async Task<List<Usuario>> BuscarPorNomeAsync(string nome)
    {
        return await _context.Usuarios
            .Where(u => u.Ativo && u.Nome.ToLower().Contains(nome.ToLower()))
            .OrderBy(u => u.Nome)
            .Take(20)
            .ToListAsync();
    }

    public async Task<ConfiguracaoPrivacidade?> ObterPrivacidadeAsync(Guid usuarioId)
    {
        return await _context.ConfiguracaoPrivacidades
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task SalvarPrivacidadeAsync(ConfiguracaoPrivacidade config)
    {
        var existente = await _context.ConfiguracaoPrivacidades
            .FirstOrDefaultAsync(c => c.UsuarioId == config.UsuarioId);

        if (existente is null)
        {
            await _context.ConfiguracaoPrivacidades.AddAsync(config);
        }
        else
        {
            existente.PerfilPublico = config.PerfilPublico;
            existente.CheckinsPublicos = config.CheckinsPublicos;
            existente.ConquistasPublicas = config.ConquistasPublicas;
            existente.NivelPublico = config.NivelPublico;
            existente.VisivelNoMapa = config.VisivelNoMapa;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Usuario>> ObterSugestoesAsync(Guid usuarioId, int limite = 10)
    {
        // IDs que já sigo
        var jaSegueIds = await _context.Seguidores
            .Where(s => s.SeguidorId == usuarioId)
            .Select(s => s.SeguidoId)
            .ToListAsync();

        var excluir = new HashSet<Guid>(jaSegueIds) { usuarioId };

        // Campings visitados pelo usuário
        var meusCampings = await _context.Checkins
            .Where(c => c.UsuarioId == usuarioId && c.CampingId != null)
            .Select(c => c.CampingId!.Value)
            .Distinct()
            .ToListAsync();

        // Usuários que visitaram os mesmos campings
        var porCamping = meusCampings.Count > 0
            ? await _context.Checkins
                .Where(c => meusCampings.Contains(c.CampingId!.Value) && !excluir.Contains(c.UsuarioId))
                .Select(c => c.UsuarioId)
                .Distinct()
                .Take(limite)
                .ToListAsync()
            : new List<Guid>();

        // Amigos de amigos: seguidos pelos meus seguidos
        var amigosDeAmigos = await _context.Seguidores
            .Where(s => jaSegueIds.Contains(s.SeguidorId) && !excluir.Contains(s.SeguidoId))
            .Select(s => s.SeguidoId)
            .Distinct()
            .Take(limite)
            .ToListAsync();

        var sugestaoIds = porCamping.Union(amigosDeAmigos).Take(limite).ToList();

        if (sugestaoIds.Count == 0)
            return [];

        return await _context.Usuarios
            .Where(u => sugestaoIds.Contains(u.Id) && u.Ativo)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<List<Usuario>> ObterSeguidoresMutuosAsync(Guid usuarioId)
    {
        // Usuários que me seguem E que eu também sigo
        return await _context.Seguidores
            .Where(s => s.SeguidoId == usuarioId &&
                        _context.Seguidores.Any(f => f.SeguidorId == usuarioId && f.SeguidoId == s.SeguidorId))
            .Select(s => s.SeguidorUsuario)
            .ToListAsync();
    }
}
