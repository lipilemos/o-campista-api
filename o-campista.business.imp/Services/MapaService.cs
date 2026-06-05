using o_campista.business.IServices;
using o_campista.repository.Repositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class MapaService : IMapaService
{
    private readonly ICampingRepository _campingRepository;

    public MapaService(
        ICampingRepository campingRepository)
    {
        _campingRepository = campingRepository;
    }

    public async Task<IEnumerable<CampingMapaResponse>>
        ObterCampingsMapaAsync()
    {
        var campings =
            await _campingRepository
                .ObterCampingsMapaAsync();

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
            FotoPrincipal =
                c.Fotos
                 .Where(f => f.Principal)
                 .Select(f => f.Url)
                 .FirstOrDefault()
                 ?? string.Empty,
            Recursos =
                c.Recursos
                 .Select(r => new RecursoCampingResponse
                 {
                     Nome = r.Recurso.Nome,
                     Disponivel = r.Disponivel
                 })
                 .ToList()
        });
    }
}