using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.IServices
{
    public interface IUsuarioTrilhaService
    {
        Task ConcluirTrilhaAsync(Guid usuarioId,long trilhaId);
    }
}
