using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface ICheckinService
    {
        Task RealizarCheckinAsync(CheckinRequest request);
        Task<List<HistoricoCheckinResponse>> ObtenerHistoricoAsync(Guid usuarioId);
        Task<int> ContarPessoasUltimas24hAsync(long campingId);
        Task RealizarCheckinTrilhaAsync(long trilhaId, Guid usuarioId, decimal latitude, decimal longitude);
        Task<int> ContarPessoasTrilhaUltimas24hAsync(long trilhaId);
        Task<int> ContarTotalVisitasCampingAsync(long campingId);
        Task<int> ContarTotalVisitasTrilhaAsync(long trilhaId);
    }
}
