using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface ICheckinRepository
    {
        Task CriarAsync(Checkin checkin);
        Task<bool> JaExisteHojeAsync(Guid usuarioId,long campingId);
        Task<int> ObterQuantidadeCheckinsUsuarioAsync(Guid usuarioId);
    }
}
