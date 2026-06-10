using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface IUsuarioPresenteRepository
    {
        Task<int> ContarResgatadosPorUsuarioAsync(Guid usuarioId);
        Task CriarAsync(UsuarioPresente usuarioPresente);

    }
}
