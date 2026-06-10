using o_campista.entities.Entities;
using o_campista.entities.Entities.o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface IUsuarioTrilhaRepository
    {
        Task<UsuarioTrilha?> ObterAsync(Guid usuarioId,long trilhaId);

        Task CriarAsync(UsuarioTrilha usuarioTrilha);

        Task AtualizarAsync(UsuarioTrilha usuarioTrilha);

        Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId);
        }
    
}
