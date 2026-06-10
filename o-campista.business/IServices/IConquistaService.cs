using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface IConquistaService
    {
        Task VerificarConquistasAsync(Guid usuarioId);
    }
}
