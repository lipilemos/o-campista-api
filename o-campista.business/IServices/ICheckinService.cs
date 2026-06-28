using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;

namespace o_campista.business.IServices
{
    public interface ICheckinService
    {
        Task RealizarCheckinAsync(CheckinRequest request);
        Task<List<HistoricoCheckinResponse>> ObtenerHistoricoAsync(Guid usuarioId);
    }
}
