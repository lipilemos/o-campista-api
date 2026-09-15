using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IAchadoPerdidoService
{
    Task<AcessoAchadosResponse> ObterAcessoAsync(Guid usuarioId, long campingId);

    Task<List<AchadoPerdidoResponse>> ListarAsync(
        Guid usuarioId, long campingId, string? tipo, bool incluirResolvidos, int pagina, int limite);

    Task<AchadoPerdidoResponse> CriarAsync(Guid usuarioId, long campingId, AchadoPerdidoRequest request);

    Task ResolverAsync(long id, Guid usuarioId);

    Task DeletarAsync(long id, Guid usuarioId);

    Task<SalaChatResponse> ReivindicarAsync(long id, Guid usuarioId);
}
