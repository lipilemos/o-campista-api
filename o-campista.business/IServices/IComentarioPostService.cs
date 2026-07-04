using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IComentarioPostService
{
    Task<ComentarioPostResponse> CriarComentarioAsync(long postId, Guid usuarioId, ComentarioPostRequest request);
    Task DeletarComentarioAsync(long comentarioId, Guid usuarioId);
    Task<List<ComentarioPostResponse>> ObterComentariosAsync(long postId, int pagina, int limite);
}
