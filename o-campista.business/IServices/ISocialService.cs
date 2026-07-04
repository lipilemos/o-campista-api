using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices;

public interface ISocialService
{
    Task<PerfilPublicoResponse> ObterPerfilPublicoAsync(Guid perfilId, Guid? requisitanteId);
    Task<List<UsuarioBuscaResponse>> BuscarUsuariosAsync(string nome);
    Task SegueAsync(Guid seguidorId, Guid seguidoId);
    Task PararDeSegueAsync(Guid seguidorId, Guid seguidoId);
    Task<List<UsuarioBuscaResponse>> ObterSeguidoresAsync(Guid usuarioId);
    Task<List<UsuarioBuscaResponse>> ObterSeguindoAsync(Guid usuarioId);
    Task<ConfiguracaoPrivacidadeResponse> ObterPrivacidadeAsync(Guid usuarioId);
    Task SalvarPrivacidadeAsync(Guid usuarioId, ConfiguracaoPrivacidadeRequest request);
    Task<List<UsuarioBuscaResponse>> ObterSugestoesAsync(Guid usuarioId);
}
