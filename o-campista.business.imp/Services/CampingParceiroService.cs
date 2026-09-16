using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class CampingParceiroService : ICampingParceiroService
{
    private const double RaioProximosMetros = 2_000d;
    private const int DiasPainel = 30;

    private readonly ICampingRepository _campingRepository;
    private readonly ICheckinRepository _checkinRepository;

    public CampingParceiroService(ICampingRepository campingRepository, ICheckinRepository checkinRepository)
    {
        _campingRepository = campingRepository;
        _checkinRepository = checkinRepository;
    }

    public async Task<CampingParceiroResponse> CriarAsync(Guid usuarioId, CampingParceiroRequest request)
    {
        var tipo = request.Tipo.Trim().ToLowerInvariant();
        if (!Camping.TiposComDono.Contains(tipo))
            throw new ArgumentException("Tipo inválido. Use 'camping' ou 'pesca'.");

        var camping = new Camping
        {
            Nome = request.Nome.Trim(),
            Tipo = tipo,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            Endereco = string.IsNullOrWhiteSpace(request.Endereco) ? null : request.Endereco.Trim(),
            Cidade = request.Cidade.Trim(),
            Estado = request.Estado.Trim().ToUpperInvariant(),
            Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Ativo = false,
            DonoUsuarioId = usuarioId,
            DonoStatus = Camping.DonoStatusPendente,
            Recursos = request.RecursosIds
                .Distinct()
                .Select(id => new CampingRecurso { RecursoId = id, Disponivel = true })
                .ToList(),
        };

        var criado = await _campingRepository.CriarAsync(camping);
        return Mapear(criado);
    }

    public async Task<CampingParceiroResponse> ReivindicarAsync(Guid usuarioId, long campingId)
    {
        var camping = await _campingRepository.ObterPorIdAsync(campingId);
        if (camping is null || !camping.Ativo || !Camping.TiposComDono.Contains(camping.Tipo))
            throw new KeyNotFoundException("Camping não encontrado.");

        if (camping.DonoUsuarioId is not null)
            throw new InvalidOperationException("Este camping já possui um dono vinculado.");

        camping.DonoUsuarioId = usuarioId;
        camping.DonoStatus = Camping.DonoStatusPendente;
        await _campingRepository.AtualizarAsync(camping);
        return Mapear(camping);
    }

    public async Task<List<CampingParceiroResponse>> ObterMeusAsync(Guid usuarioId)
    {
        var campings = await _campingRepository.ObterPorDonoAsync(usuarioId);
        return campings.Select(Mapear).ToList();
    }

    public async Task<List<CampingProximoResponse>> ObterProximosSemDonoAsync(decimal latitude, decimal longitude)
    {
        var proximos = await _campingRepository.ObterSemDonoNoRaioAsync(latitude, longitude, RaioProximosMetros);
        return proximos.Select(p => new CampingProximoResponse
        {
            Id = p.Camping.Id,
            Nome = p.Camping.Nome,
            Tipo = p.Camping.Tipo,
            Cidade = p.Camping.Cidade ?? string.Empty,
            Estado = p.Camping.Estado ?? string.Empty,
            DistanciaMetros = Math.Round(p.DistanciaMetros),
        }).ToList();
    }

    public async Task<CampingPainelResponse> ObterPainelAsync(Guid usuarioId, long campingId)
    {
        var camping = await _campingRepository.ObterPorIdAsync(campingId)
            ?? throw new KeyNotFoundException("Camping não encontrado.");

        if (camping.DonoUsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não é o dono deste camping.");

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var desde = DateTime.UtcNow.Date.AddDays(-(DiasPainel - 1));

        var porDia = await _checkinRepository.ObterCheckinsPorDiaAsync(campingId, desde);
        var ocupacoes = await _checkinRepository.ObterStatusOcupacaoTodosAsync();

        return new CampingPainelResponse
        {
            CampingId = campingId,
            Checkins30Dias = await _checkinRepository.ContarCheckinsPeriodoAsync(campingId, desde),
            CheckinsTotal = await _checkinRepository.ContarTotalCheckinsCampingAsync(campingId),
            VisitantesUnicos = await _checkinRepository.ContarVisitantesUnicosAsync(campingId),
            AvaliacaoMedia = camping.AvaliacaoMedia,
            TotalAvaliacoes = await _campingRepository.ContarAvaliacoesAsync(campingId),
            TotalFavoritos = await _campingRepository.ContarFavoritosAsync(campingId),
            StatusOcupacao = ocupacoes.GetValueOrDefault(campingId),
            CheckinsPorDia = Enumerable.Range(0, DiasPainel)
                .Select(i => hoje.AddDays(-(DiasPainel - 1 - i)))
                .Select(dia => new CheckinsDiaResponse
                {
                    Data = dia,
                    Quantidade = porDia.GetValueOrDefault(dia),
                })
                .ToList(),
        };
    }

    private static CampingParceiroResponse Mapear(Camping c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Tipo = c.Tipo,
        Cidade = c.Cidade ?? string.Empty,
        Estado = c.Estado ?? string.Empty,
        Latitude = c.Latitude,
        Longitude = c.Longitude,
        Ativo = c.Ativo,
        DonoStatus = c.DonoStatus ?? string.Empty,
        CriadoEm = c.CriadoEm,
    };
}
