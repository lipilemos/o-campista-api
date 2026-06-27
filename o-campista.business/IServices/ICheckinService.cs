using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface ICheckinService
    {
        Task RealizarCheckinAsync(CheckinRequest request);
        Task<List<HistoricoCheckinResponse>> ObtenerHistoricoAsync(Guid usuarioId);
    }
}
