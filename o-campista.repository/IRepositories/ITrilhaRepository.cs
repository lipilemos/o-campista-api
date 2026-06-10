using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface ITrilhaRepository
    {
        Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId);
    }
}
