using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class MapaService : IMapaService
{
    private readonly ICampingRepository _campingRepository;
    private readonly ICheckinRepository _checkinRepository;

    public MapaService(
        ICampingRepository campingRepository,
        ICheckinRepository checkinRepository)
    {
        _campingRepository = campingRepository;
        _checkinRepository = checkinRepository;
    }

    public async Task<IEnumerable<CampingMapaResponse>>
        ObterCampingsMapaAsync(string? busca = null, string? tipo = null, string[]? recursos = null)
    {
        var campings = await _campingRepository.ObterCampingsMapaAsync(busca, tipo, recursos);
        var statusOcupacao = await _checkinRepository.ObterStatusOcupacaoTodosAsync();

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
                 .ToList(),
            StatusOcupacao = statusOcupacao.TryGetValue(c.Id, out var status) ? status : null
        });
    }
}
