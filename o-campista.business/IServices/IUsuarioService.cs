using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface IUsuarioService
    {
        Task<LoginResponse> ObterPerfilAsync(Guid usuarioId);
        Task AdicionarXPAsync(Guid usuarioId, int xp);
    }
}