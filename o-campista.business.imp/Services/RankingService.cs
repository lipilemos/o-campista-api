using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class RankingService : IRankingService
{
    private readonly IRankingRepository _rankingRepository;
    private readonly ISocialRepository _socialRepository;
    private readonly ILogger<RankingService> _logger;

    public RankingService(
        IRankingRepository rankingRepository,
        ISocialRepository socialRepository,
        ILogger<RankingService> logger)
    {
        _rankingRepository = rankingRepository;
        _socialRepository = socialRepository;
        _logger = logger;
    }

    public async Task<List<RankingItemResponse>> ObterRankingGlobalAsync(Guid usuarioId, int pagina, int limite)
    {
        _logger.LogInformation("Buscando ranking global. Usuario={UsuarioId} Pagina={Pagina}", usuarioId, pagina);

        var skip = (pagina - 1) * limite;
        var itens = await _rankingRepository.ObterRankingGlobalAsync(skip, limite);
        var seguindo = await _socialRepository.ObterSeguindoAsync(usuarioId);
        var seguindoIds = seguindo.Select(u => u.Id).ToHashSet();

        return itens.Select((item, index) => new RankingItemResponse
        {
            Posicao = skip + index + 1,
            UsuarioId = item.Id,
            Nome = item.Nome,
            FotoPerfil = item.FotoPerfil,
            Nivel = item.Nivel,
            Xp = item.Xp,
            TotalCheckins = item.TotalCheckins,
            EstouSeguindo = seguindoIds.Contains(item.Id)
        }).ToList();
    }

    public async Task<List<RankingItemResponse>> ObterRankingSeguidosAsync(Guid usuarioId)
    {
        _logger.LogInformation("Buscando ranking de seguidos. Usuario={UsuarioId}", usuarioId);

        var seguindo = await _socialRepository.ObterSeguindoAsync(usuarioId);
        if (seguindo.Count == 0) return [];

        var seguidoIds = seguindo.Select(u => u.Id).ToList();
        var itens = await _rankingRepository.ObterRankingPorIdsAsync(seguidoIds);

        return itens.Select((item, index) => new RankingItemResponse
        {
            Posicao = index + 1,
            UsuarioId = item.Id,
            Nome = item.Nome,
            FotoPerfil = item.FotoPerfil,
            Nivel = item.Nivel,
            Xp = item.Xp,
            TotalCheckins = item.TotalCheckins,
            EstouSeguindo = true
        }).ToList();
    }

    public async Task<List<CampingRankingResponse>> ObterRankingCampingsAsync(int pagina, int limite)
    {
        _logger.LogInformation("Buscando ranking de campings. Pagina={Pagina}", pagina);

        var skip = (pagina - 1) * limite;
        var itens = await _rankingRepository.ObterRankingCampingsAsync(skip, limite);

        return itens.Select((item, index) => new CampingRankingResponse
        {
            Posicao = skip + index + 1,
            CampingId = item.Id,
            Nome = item.Nome,
            Cidade = item.Cidade,
            Estado = item.Estado,
            FotoPrincipal = item.FotoPrincipal,
            AvaliacaoMedia = item.AvaliacaoMedia,
            TotalCheckins = item.TotalCheckins
        }).ToList();
    }
}
