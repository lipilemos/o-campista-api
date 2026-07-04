using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class FavoritoCampingService : IFavoritoCampingService
{
    private readonly IFavoritoCampingRepository _favoritoRepository;

    public FavoritoCampingService(IFavoritoCampingRepository favoritoRepository)
    {
        _favoritoRepository = favoritoRepository;
    }

    public async Task FavoritarAsync(Guid usuarioId, long campingId)
    {
        var jaExiste = await _favoritoRepository.ExisteAsync(usuarioId, campingId);
        if (jaExiste) return;

        await _favoritoRepository.AdicionarAsync(new UsuarioCampingFavorito
        {
            UsuarioId = usuarioId,
            CampingId = campingId,
            CriadoEm = DateTime.UtcNow,
        });
    }

    public async Task DesfavoritarAsync(Guid usuarioId, long campingId)
    {
        await _favoritoRepository.RemoverAsync(usuarioId, campingId);
    }

    public async Task<List<CampingMapaResponse>> ObterFavoritosAsync(Guid usuarioId)
    {
        var campings = await _favoritoRepository.ObterPorUsuarioAsync(usuarioId);
        return campings.Select(c => new CampingMapaResponse
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao ?? string.Empty,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            Cidade = c.Cidade ?? string.Empty,
            Estado = c.Estado ?? string.Empty,
            Endereco = c.Endereco ?? string.Empty,
            Telefone = c.Telefone ?? string.Empty,
            Tipo = c.Tipo,
            Avaliacao = (double)c.AvaliacaoMedia,
            FotoPrincipal = c.Fotos
                .Where(f => f.Principal)
                .Select(f => f.Url)
                .FirstOrDefault() ?? string.Empty,
            Recursos = c.Recursos
                .Select(r => new RecursoCampingResponse
                {
                    Nome = r.Recurso.Nome,
                    Disponivel = r.Disponivel,
                })
                .ToList(),
        }).ToList();
    }
}
