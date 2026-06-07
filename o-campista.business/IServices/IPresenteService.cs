using o_campista.entities.Entities;
using o_campista.shared.Models.Requests;
using o_campista.shared.Models.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface IPresenteService
    {
        Task<Presente> CriarNovoPresenteAsync(PresenteCreateRequest request);
        Task<IEnumerable<PresenteResponse>> ObterPresentesPorRaioAsync(double latitude, double longitude);
    }
}
