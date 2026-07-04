using o_campista.entities.Entities;

namespace o_campista.repository.IRepositories;

public interface IFeedRepository
{
    Task CriarAtividadeAsync(AtividadeFeed atividade);
    Task<List<AtividadeFeed>> ObterAtividadesPorUsuariosAsync(IEnumerable<Guid> usuarioIds, int skip, int take);
    Task<List<AtividadeFeed>> ObterAtividadesPublicasAsync(int skip, int take);

    // Enrichment helpers — batch load referenced entities
    Task<List<Checkin>> ObterCheckinsPorIdsAsync(IEnumerable<long> ids);
    Task<List<CampingAvaliacao>> ObterAvaliacoesPorIdsAsync(IEnumerable<long> ids);
    Task<List<Conquista>> ObterConquistasPorIdsAsync(IEnumerable<long> ids);
    Task<List<PostViagem>> ObterPostsPorIdsAsync(IEnumerable<long> ids);
    Task<List<Trilha>> ObterTrilhasPorIdsAsync(IEnumerable<long> ids);
    Task<Dictionary<long, int>> ObterCurtidasTotaisPorPostIdsAsync(IEnumerable<long> postIds);
    Task<List<long>> ObterPostsCurtidosPeloUsuarioAsync(Guid usuarioId, IEnumerable<long> postIds);
    Task<Dictionary<long, int>> ObterComentariosTotaisPorPostIdsAsync(IEnumerable<long> postIds);
}
