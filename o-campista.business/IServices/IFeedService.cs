using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IFeedService
{
    Task<List<FeedItemResponse>> ObterFeedAsync(Guid usuarioId, int pagina, int limite);
    Task<List<FeedItemResponse>> ObterDescobrirAsync(Guid? usuarioId, int pagina, int limite);
    Task CriarAtividadeAsync(Guid usuarioId, string tipo, long referenciaId);
}
