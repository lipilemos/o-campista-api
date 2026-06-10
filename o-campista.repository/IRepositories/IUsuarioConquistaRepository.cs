using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface IUsuarioConquistaRepository
    {
        Task<bool> ExisteAsync(Guid usuarioId,long conquistaId);
        Task CriarAsync(UsuarioConquista conquista);
    }
}
