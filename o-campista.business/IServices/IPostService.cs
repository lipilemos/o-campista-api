using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface IPostService
{
    Task<PostViagemResponse> CriarPostAsync(Guid usuarioId, PostViagemRequest request);
    Task DeletarPostAsync(long postId, Guid usuarioId);
    Task CurtirAsync(long postId, Guid usuarioId);
    Task DescurtirAsync(long postId, Guid usuarioId);
    Task<CurtidasPostResponse> ObterCurtidasAsync(long postId, Guid usuarioId);
}
