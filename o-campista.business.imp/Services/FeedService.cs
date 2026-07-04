using Microsoft.Extensions.Logging;
using o_campista.business.IServices;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using o_campista.shared.Models.Responses;

namespace o_campista.business.imp.Services;

public class FeedService : IFeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly ISocialRepository _socialRepository;
    private readonly ILogger<FeedService> _logger;

    public FeedService(
        IFeedRepository feedRepository,
        ISocialRepository socialRepository,
        ILogger<FeedService> logger)
    {
        _feedRepository = feedRepository;
        _socialRepository = socialRepository;
        _logger = logger;
    }

    public async Task<List<FeedItemResponse>> ObterFeedAsync(Guid usuarioId, int pagina, int limite)
    {
        var seguidos = await _socialRepository.ObterSeguindoAsync(usuarioId);
        if (seguidos.Count == 0)
            return [];

        var seguidoIds = seguidos.Select(u => u.Id).ToList();
        var skip = (pagina - 1) * limite;

        var atividades = await _feedRepository.ObterAtividadesPorUsuariosAsync(seguidoIds, skip, limite);
        return await EnriquecerAtividadesAsync(atividades, usuarioId);
    }

    public async Task<List<FeedItemResponse>> ObterDescobrirAsync(Guid? usuarioId, int pagina, int limite)
    {
        var skip = (pagina - 1) * limite;
        var atividades = await _feedRepository.ObterAtividadesPublicasAsync(skip, limite);
        return await EnriquecerAtividadesAsync(atividades, usuarioId);
    }

    public async Task CriarAtividadeAsync(Guid usuarioId, string tipo, long referenciaId)
    {
        try
        {
            var atividade = new AtividadeFeed
            {
                UsuarioId = usuarioId,
                Tipo = tipo,
                ReferenciaId = referenciaId,
                CriadoEm = DateTime.UtcNow,
                Visivel = true
            };
            await _feedRepository.CriarAtividadeAsync(atividade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar atividade de feed. Usuario={UsuarioId} Tipo={Tipo}", usuarioId, tipo);
        }
    }

    private async Task<List<FeedItemResponse>> EnriquecerAtividadesAsync(
        List<AtividadeFeed> atividades, Guid? usuarioId)
    {
        if (atividades.Count == 0)
            return [];

        var porTipo = atividades.GroupBy(a => a.Tipo).ToDictionary(g => g.Key, g => g.ToList());

        var checkins = porTipo.TryGetValue("checkin", out var ac)
            ? await _feedRepository.ObterCheckinsPorIdsAsync(ac.Select(a => a.ReferenciaId))
            : [];

        var avaliacoes = porTipo.TryGetValue("avaliacao", out var aa)
            ? await _feedRepository.ObterAvaliacoesPorIdsAsync(aa.Select(a => a.ReferenciaId))
            : [];

        var conquistas = porTipo.TryGetValue("conquista", out var aq)
            ? await _feedRepository.ObterConquistasPorIdsAsync(aq.Select(a => a.ReferenciaId))
            : [];

        var postIds = porTipo.TryGetValue("post", out var ap)
            ? ap.Select(a => a.ReferenciaId).ToList()
            : [];

        var posts = postIds.Count > 0
            ? await _feedRepository.ObterPostsPorIdsAsync(postIds)
            : [];

        var trilhas = porTipo.TryGetValue("trilha_concluida", out var at)
            ? await _feedRepository.ObterTrilhasPorIdsAsync(at.Select(a => a.ReferenciaId))
            : [];

        Dictionary<long, int> curtidasTotais = [];
        Dictionary<long, int> comentariosTotais = [];
        List<long> curtidasDoUsuario = [];
        if (postIds.Count > 0)
        {
            curtidasTotais = await _feedRepository.ObterCurtidasTotaisPorPostIdsAsync(postIds);
            comentariosTotais = await _feedRepository.ObterComentariosTotaisPorPostIdsAsync(postIds);
            if (usuarioId.HasValue)
                curtidasDoUsuario = await _feedRepository.ObterPostsCurtidosPeloUsuarioAsync(usuarioId.Value, postIds);
        }

        var checkinsDict = checkins.ToDictionary(c => c.Id);
        var avaliacoesDict = avaliacoes.ToDictionary(a => a.Id);
        var conquistasDict = conquistas.ToDictionary(c => c.Id);
        var postsDict = posts.ToDictionary(p => p.Id);
        var trilhasDict = trilhas.ToDictionary(t => t.Id);

        return atividades.Select(a => MapToResponse(
            a, checkinsDict, avaliacoesDict, conquistasDict, postsDict, trilhasDict,
            curtidasTotais, comentariosTotais, curtidasDoUsuario)).ToList();
    }

    private static FeedItemResponse MapToResponse(
        AtividadeFeed a,
        Dictionary<long, Checkin> checkins,
        Dictionary<long, CampingAvaliacao> avaliacoes,
        Dictionary<long, Conquista> conquistas,
        Dictionary<long, PostViagem> posts,
        Dictionary<long, Trilha> trilhas,
        Dictionary<long, int> curtidasTotais,
        Dictionary<long, int> comentariosTotais,
        List<long> curtidasDoUsuario)
    {
        var item = new FeedItemResponse
        {
            Id = a.Id,
            Tipo = a.Tipo,
            UsuarioId = a.UsuarioId,
            UsuarioNome = a.Usuario.Nome,
            UsuarioFoto = a.Usuario.FotoPerfil,
            CriadoEm = a.CriadoEm
        };

        switch (a.Tipo)
        {
            case "checkin" when checkins.TryGetValue(a.ReferenciaId, out var c):
                item.CampingId = c.CampingId;
                item.CampingNome = c.Camping?.Nome;
                item.CampingFoto = c.Camping?.Fotos.FirstOrDefault()?.Url;
                item.OcupacaoReportada = c.Ocupacao;
                break;

            case "conquista" when conquistas.TryGetValue(a.ReferenciaId, out var cq):
                item.ConquistaNome = cq.Nome;
                item.ConquistaDescricao = cq.Descricao;
                item.ConquistaIcone = cq.Icone;
                break;

            case "avaliacao" when avaliacoes.TryGetValue(a.ReferenciaId, out var av):
                item.Nota = av.Nota;
                item.Comentario = av.Comentario;
                item.AvaliacaoFotoUrl = av.FotoUrl;
                item.CampingId = av.CampingId;
                item.LocalNome = av.Camping?.Nome ?? av.Trilha?.Nome;
                break;

            case "post" when posts.TryGetValue(a.ReferenciaId, out var p):
                item.PostId = p.Id;
                item.PostTexto = p.Texto;
                item.PostFotoUrl = p.FotoUrl;
                curtidasTotais.TryGetValue(p.Id, out var total);
                item.TotalCurtidas = total;
                comentariosTotais.TryGetValue(p.Id, out var totalComentarios);
                item.TotalComentarios = totalComentarios;
                item.Curtiu = curtidasDoUsuario.Contains(p.Id);
                item.CampingId = p.CampingId;
                item.CampingNome = p.Camping?.Nome;
                break;

            case "trilha_concluida" when trilhas.TryGetValue(a.ReferenciaId, out var t):
                item.TrilhaNome = t.Nome;
                item.DistanciaKm = t.DistanciaKm;
                item.Dificuldade = t.Dificuldade;
                break;
        }

        return item;
    }
}
