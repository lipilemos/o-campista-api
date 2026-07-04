using o_campista.entities.Entities;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface ISalaChatService
    {
        Task<List<SalaChatResponse>> ObterSalasAsync(Guid usuarioId);
        Task<List<MensagemSalaChatResponse>> ObterMensagensAsync(long salaId, Guid usuarioId, int skip, int take);
        Task<CriarGrupoResponse> CriarGrupoAsync(Guid usuarioId, CriarGrupoRequest request);
        Task<SalaChatResponse> EntrarGrupoAsync(Guid usuarioId, string codigoConvite);
        Task<string> ObterConviteAsync(long salaId, Guid usuarioId);
        Task<List<MembroSalaResponse>> ObterMembrosAsync(long salaId);
        Task MarcarComoLidaAsync(long salaId, Guid usuarioId);
        Task<NaoLidasResponse> ObterNaoLidasAsync(Guid usuarioId);
        Task<PodeEnviarResponse> PodeEnviarAsync(long salaId, Guid usuarioId);
        Task<MensagemSalaChatResponse> SalvarMensagemAsync(long salaId, Guid usuarioId, string texto);
        Task<SalaChat> CriarSalaCampingSeNaoExisteAsync(long campingId, Guid usuarioId);
        Task SairDoGrupoAsync(long salaId, Guid usuarioId);
        Task<SalaChatResponse> ObterOuCriarDmAsync(Guid solicitanteId, Guid destinatarioId);
    }
}
