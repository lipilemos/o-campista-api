using o_campista.shared.Models.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface ICheckinService
    {
        Task RealizarCheckinAsync(CheckinRequest request);
    }
}
